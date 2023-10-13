namespace Etl

open FSharp.Data.Sql
open Db
open Models
open Tools
open Literals
open Etl
open DbInsertService
open FileService
open LteDssNr
open Gsm
open Umts
open System

module EtlAddTech =

    let getParamForMissTechCheck (sitename: string, tech: Technology, rows: Common.SqlEntity seq) =
        rows
        |> Seq.filter (fun row -> 
                colSiteName.Get row = sitename)
        |> Seq.map (fun row ->
            { SiteName = colSiteName.ColumnName
              Sector = getSectorFromCellName (colTxId.Get row)
              CellNameLastDigit = getCellNameLastDigit (colTxId.Get row)
              Fband = colFband.Get row
              Tech =
                match colTxId.Get row with
                | x when getCellNameLastDigit x = 'F' -> Technology.DSS
                | x when getCellNameLastDigit x = 'N' -> Technology.NR
                | _ -> tech })


    let private allTransSiteRef = 
                getParamForMissTechCheck(SiteNameRef, Technology.GSM, gtransCloneGeneric)
                |> Seq.append (getParamForMissTechCheck(SiteNameRef, Technology.UMTS, utransCloneGeneric))
                |> Seq.append (getParamForMissTechCheck(SiteNameRef, Technology.LTE, xgtransCloneGeneric))
   

    let getMissTech  (siteNameOption:string option) =
        match siteNameOption with
        |Some siteName -> (
            let allTransSite = 
                getParamForMissTechCheck(siteName, Technology.GSM, gtransOrigGeneric)
                |> Seq.append (getParamForMissTechCheck(siteName, Technology.UMTS, utransOrigGeneric))
                |> Seq.append (getParamForMissTechCheck(siteName, Technology.LTE, xgtransOrigGeneric))
        
            let allTransSiteRefWhereSectorExistInSite = 
                allTransSiteRef
                |>Seq.filter(fun x-> 
                        allTransSite
                        |>Seq.map(fun x-> x.Sector)
                        |>Seq.contains x.Sector)
        
            //log $"-----Get Miss Tech {siteName}-----"

            allTransSiteRefWhereSectorExistInSite 
            |> Seq.except allTransSite
            |> fun missTech ->
                match missTech with
                | x when Seq.isEmpty x -> (
                    log  $"No new tech to add- {siteName}"
                    List.empty)
                | _ -> missTech
                       |>Seq.map(fun row-> {row with SiteName=siteName})
                       |>Seq.sortBy(fun row-> fbandToBand row.Fband)
                       |>List.ofSeq )
        |None-> []


    let techToAdd(paramForAdd : ParamForAdd) =

        let transObj = paramObjToTransObj(paramForAdd)

        let toAdd = 
            match paramForAdd.Tech with
            |Technology.GSM -> transObj |> gsmToAdd
            |Technology.UMTS -> transObj|> umtsToAdd
            |Technology.LTE |Technology.DSS |Technology.NR -> 
                                transObj|> lteDssNrToAdd
            | _ ->raise (Exception($"Error: No such tech {paramForAdd.Tech}!"))

        log $"-----Add Tech {paramForAdd.SiteName}-----"

        toAdd |>List.ofSeq
        |> List.iter(fun x-> (
            log $"Adding {x.EntityName}:{transObj.SiteName}:{transObj.TxId}:{fbandToBand transObj.Fband}...."
            try
                insertToDb  x.Entity
            with
            |ex  -> 
                log ex.Message
                saveToErrorFile paramForAdd.SiteName ex.Message
            ))

