namespace Etl

open Models
open System
open System.IO
open Literals
open FSharp.Data.Sql.Common
open System.Text.RegularExpressions
open FSharp.Data.Sql
open System.Diagnostics
open FSharp.Data.SqlClient.Internals
open System.Data
open Db
open FileService

module Tools =

    let colSiteName = { ColumnName = "SITE_NAME" }
    let colPairedCellName = { ColumnName = "PAIRED_CELL_NAME" }
    let colTxIdOther = { ColumnName = "TX_ID_OTHER" }
    let colTxId = { ColumnName = "TX_ID" }
    let colCellId = { ColumnName = "CELL_ID" }
    let colTrans = { ColumnName = "TRANSMITTER" }
    let colNeigh = { ColumnName = "NEIGHBOUR" }
    let colDonorCellId = { ColumnName = "DONOR_CELLID" }
    let colTxName = { ColumnName = "TX_NAME" }
    let colUniqueId = { ColumnName = "UNIQUE_ID" }
    let colHeight = { ColumnName = "HEIGHT" }
    let colAzimuth = { ColumnName = "AZIMUT" }
    let colTilt = { ColumnName = "TILT" }
    let colSiteProgress = { ColumnName = "CF_SITE_PROGRESS" }
    let colParamPlan = { ColumnName = "CF_PARAMETERS_PLAN" }
    let colAntName = { ColumnName = "ANTENNA_NAME" }
    let colFband = { ColumnName = "FBAND" }
    let colCellIdentity = { ColumnName = "CELL_IDENTITY" }
    let colCfNodebId = { ColumnName = "CF_NODEB_ID" }
    let colName = { ColumnName = "NAME" }
    let colSiteType = { ColumnName = "CF_SITE_TYPE" }
    let colCandidate = { ColumnName = "CF_CANDIDATE" }


    let castObjectToBytes (obj: obj) : byte seq option =
        match obj with
        | :? (byte seq) as bytes -> Some bytes
        | _ -> None


    let objOfBytesToString (obj: obj) =
        let bytes = (castObjectToBytes obj).Value
        let bytesToStrSeq = bytes |> Seq.map (fun x -> x.ToString("X2"))
        let bytesToStr = String.Join("", bytesToStrSeq)
        $"0x{bytesToStr}"


    let getSectorFromCellName (cellName: string) = cellName.[4] |> string


    let getCellNameLastDigit (cellName: string) = cellName |> Seq.last


    let getCellIdFromCellName (cellName: string) =
        let cellId = cellName.Remove(cellName.Length - 1)

        match Int32.TryParse cellName with
        | true, b -> b
        | false, _ -> 0


    let bandFreq =
        [ {| Tech = Technology.GSM
             Band = Band.B9
             Fband = "V GSM 900 E" |}
          {| Tech = Technology.GSM
             Band = Band.B18
             Fband = "V GSM 1800" |}
          {| Tech = Technology.GSM
             Band = Band.B18
             Fband = "V GSM 1800_1800" |}
          {| Tech = Technology.UMTS
             Band = Band.B9
             Fband = "UTRA Band VIII" |}
          {| Tech = Technology.UMTS
             Band = Band.B21
             Fband = "UTRA Band I" |}
          {| Tech = Technology.LTE
             Band = Band.B7
             Fband = "n28 / E-UTRA 28" |}
          {| Tech = Technology.LTE
             Band = Band.B8
             Fband = "n20 / E-UTRA 20" |}
          {| Tech = Technology.LTE
             Band = Band.B9
             Fband = "n8 / E-UTRA 8" |}
          {| Tech = Technology.LTE
             Band = Band.B18
             Fband = "n3 / E-UTRA 3" |}
          {| Tech = Technology.DSS
             Band = Band.B18
             Fband = "n3 / E-UTRA 3" |}
          {| Tech = Technology.LTE
             Band = Band.B21
             Fband = "n1 / E-UTRA 1" |}
          {| Tech = Technology.LTE
             Band = Band.B26
             Fband = "n7 / E-UTRA 7" |}
          {| Tech = Technology.NR
             Band = Band.B36
             Fband = "n78" |} ]


    let fbandToBand fband =
        let result = 
            bandFreq 
            |> Seq.find (fun x -> 
                    x.Fband = fband)
        result.Band


    let bandToFband band tech =
        let result = 
            bandFreq 
            |> (Seq.find (fun x -> 
                    x.Band = band && x.Tech = tech))
        result.Fband


    let getSiteDigit (siteName: string) = siteName[2..5]


    let siteDigitRef = getSiteDigit SiteNameRef


    let getFirstFourDigits (inputString: string) =
        let pattern = @"(^\d{4})"

        match Regex.Match(inputString, pattern) with
        | m when m.Success -> m.Groups.[1].Value
        | _ -> ""


    let antFreqToAntBand (freq: float option) =
        match freq with
        | Some f when f > 925 && f < 970 -> Some(enum<Band> 900)
        | Some f when f > 1700 && f < 1890 -> Some(enum<Band> 1800)
        | Some f when f > 1930 && f < 2170 -> Some(enum<Band> 2100)
        | Some f when f > 2600 && f < 2700 -> Some(enum<Band> 2600)
        | Some f when f > 3500 && f < 3600 -> Some(enum<Band> 3600)
        | _ -> None


    let getTableName (entities: Common.SqlEntity seq) =
        entities 
        |> Seq.map (fun row -> row.Table.FullName) 
        |> Seq.head


    let sequenceToString sequence =
        let delimiter = ", "
        let convertedSequence = Seq.map string sequence
        String.concat delimiter convertedSequence


    let commonMsgForNewItems items =
        log $"""Found {items |> Seq.length} items :
                {(String.concat "," items)}!"""
        items|>List.ofSeq


    let findNeighbourFreq freqToAdd freqs =
        let techFromFreq fband  = 
            bandFreq
            |>Seq.filter(fun f-> f.Fband=fband)
            |>Seq.map(fun f-> f.Tech)
            |>Seq.head

        let techToAdd = techFromFreq   freqToAdd

        if Seq.contains freqToAdd freqs then
            freqToAdd
        else
            let techFreqSeq = 
                freqs 
                |> Seq.map (fun freq-> ((techFromFreq   freq),freq))
                |> Seq.append [ (techToAdd,freqToAdd) ] 
                |> Seq.sortBy(fun (_,freq)-> fbandToBand freq)

            let isBandToAddIsFirst = techFreqSeq |> Seq.head = (techToAdd, freqToAdd) 

            if isBandToAddIsFirst then
                techFreqSeq 
                |> Seq.skip 1 
                |> Seq.head    
                |> function (tech,freq) -> freq
            else
                techFreqSeq 
                |> Seq.findIndex (fun (tech,freq) -> 
                    (tech, freq) = (techToAdd, freqToAdd))
                |> function index -> 
                            techFreqSeq|>List.ofSeq
                            |>function lst->
                                       let _, freq = lst.[index-1]
                                       freq


    let logAndSaveToSiteErrorJson siteName err =
                log err
                saveToErrorFile siteName err


    let getCommonParamFromTrans (rows: Common.SqlEntity seq) =
        rows
        |> Seq.map (fun row ->
            { SiteName = colSiteName.Get row
              TxId = colTxId.Get row
              Fband = colFband.Get row
              AntennaName = Option.ofObj (colAntName.Get row)
              Tilt = Some(colTilt.Get row)
              Azimut = Some(colAzimuth.Get row)
              Height = Some(colHeight.Get row)
              CfSiteProgress = Option.ofObj (colSiteProgress.Get row)
              CfParametersPlan = Option.ofObj (colParamPlan.Get row) })


    let getCommonParamAllTransCloneDb = 
        [getCommonParamFromTrans gtransCloneGeneric;
         getCommonParamFromTrans utransCloneGeneric;
         getCommonParamFromTrans xgtransCloneGeneric]
        |>Seq.collect id

    let getCommonParamAllTransOrigDb  =
        [getCommonParamFromTrans gtransOrigGeneric;
         getCommonParamFromTrans utransOrigGeneric;
         getCommonParamFromTrans xgtransOrigGeneric]
        |>Seq.collect id

     

    let mapTransParamToSqlEntity (transParam: TransParam, entity: SqlEntity) =
        let singleOptionSetDefValue value =
            value |> Option.defaultWith (fun () -> single 0)

        let strOptionSetDefValue value =
            value |> Option.defaultWith (fun () -> "None")

        [ colSiteName.Set transParam.SiteName
          colTxId.Set transParam.TxId
          colHeight.Set(transParam.Height |> singleOptionSetDefValue)
          colAzimuth.Set(transParam.Azimut |> singleOptionSetDefValue)
          colTilt.Set(transParam.Tilt |> singleOptionSetDefValue)
          colSiteProgress.Set(transParam.CfSiteProgress |> strOptionSetDefValue)
          colParamPlan.Set(transParam.CfParametersPlan |> strOptionSetDefValue)
          colAntName.Set(transParam.AntennaName |> strOptionSetDefValue) ]
        |> List.iter (fun x -> x entity)

        entity

    let mapColNameTableNames = 
            [
             (colName,["sites"])
             (colSiteName,[
                                         "xgtransmitters"
                                         "utransmitters"
                                         "gtransmitters"
                                         "siteslists"
                                             ])
             (colDonorCellId,[
                                         "xgrepeaters"
                                         "urepeaters"
                                         "grepeaters"
                                             ])
             (colTxIdOther,["upscforbidpairs"])
             (colTxId,[                 
                                         "xgtransmitters"
                                         "utransmitters"
                                         "gtransmitters"
                                         "xgcellsuids"
                                         "xgsecondaryantennas"
                                         "usecondaryantennas"
                                         "gsecondaryantennas"
                                         "ucells"
                                         "gtrxs"
                                         "gtrgs"
                                         ])
             (colPairedCellName,["xgcells5gnr"])
             //(colUniqueId,["dasda"])
             (colCellId,["xgcells5gnr";"xgcellslte"])
             (colTxName,["gtxslists"])
             (colTrans,[
                                         "guneighbours";
                                         "xguneighbours";
                                         "xggneighbours";
                                         "xgneighbours";
                                         "uneighbours";
                                         "guneighbours";
                                         "gneighbours";
                                         "ugneighbours";
                                         "gneighconstraints";
                                         "gxgneighbours";
                                         ])
             (colNeigh,[
                                         "guneighbours";
                                         "xguneighbours";
                                         "xggneighbours";
                                         "xgneighbours";
                                         "uneighbours";
                                         "guneighbours";
                                         "gneighbours";
                                         "ugneighbours";
                                         "gneighconstraints";
                                         "gxgneighbours";
                                         ])
            ]