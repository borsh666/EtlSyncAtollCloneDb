namespace Etl

open FSharp.Data.Sql
open Db
open Models
open Tools
open Literals
open FSharp.Collections.ParallelSeq
open DbDeleteService
open DbInsertService
open FileService
open IsEtlPossible
open Filters

module Etl =


    let private getCommonParamAllTransSiteRefCloneDb = 
        getCommonParamAllTransCloneDb
        |>Seq.filter (fun row -> row.SiteName = SiteNameRef)
  

    let paramObjToTransObj(paramForAdd : ParamForAdd) =
        //Reference trans -> SF7000A - one row -> per CellName
        //Will be use for add new tech on orig site

        let trans = 
            getCommonParamAllTransSiteRefCloneDb
            |>Seq.find(fun x-> 
                        getCellNameLastDigit x.TxId = paramForAdd.CellNameLastDigit &&
                        getSectorFromCellName x.TxId = paramForAdd.Sector)
          
        //trans seq -> from orig site take all trans by sector and band
        let transOrigSite = 
            let filterBySiteNameAndSector = 
                getCommonParamAllTransOrigDb 
                |>PSeq.filter(fun x-> x.SiteName = paramForAdd.SiteName &&
                                       getSectorFromCellName x.TxId = paramForAdd.Sector)
                |>List.ofSeq

            let availFreq = 
                filterBySiteNameAndSector
                |>List.map(fun x->x.Fband)

            let filterBySiteNameSectorAndFreq = 
                filterBySiteNameAndSector 
                |>Seq.filter(fun x-> 
                    x.Fband = findNeighbourFreq paramForAdd.Fband availFreq)
                |>Seq.sortBy(fun x -> x.AntennaName)
                |>Seq.sortByDescending(fun x->
                    (bandFreq
                    |>Seq.find(fun f->
                                f.Fband = x.Fband)).Band)
                |>List.ofSeq

            match filterBySiteNameSectorAndFreq with 
            | [] -> filterBySiteNameSectorAndFreq
            | _ -> filterBySiteNameSectorAndFreq

        let transOrigSiteFirst = transOrigSite|>Seq.head

        //Check for suitable anntena to place new tech
        let okAntennas = 
            transOrigSite
            |>Seq.filter(fun x-> x.AntennaName.IsSome)
            |>Seq.collect(fun x-> isAntCapable x.AntennaName.Value (fbandToBand paramForAdd.Fband))

        let transForAdd =  
            {trans with SiteName=paramForAdd.SiteName;
                        TxId = paramForAdd.GetCellName;
                         Height = transOrigSiteFirst.Height;
                         Azimut = transOrigSiteFirst.Azimut;
                         Tilt = transOrigSiteFirst.Tilt;
                         CfSiteProgress = transOrigSiteFirst.CfSiteProgress;
                         CfParametersPlan = transOrigSiteFirst.CfParametersPlan}
        
        if not (Seq.isEmpty okAntennas) then
            let okAnt = Seq.head okAntennas
            {transForAdd with AntennaName = Some(okAnt.Pattern)}
        else
            transForAdd
    
    let etl sqlCmd siteName = 
        
        let getAllEntityForSiteNameForOneTable (entities:Common.SqlEntity seq)   =
            entities
            |>PSeq.filter (filterEntityBySiteName siteName)
            |>List.ofSeq
            
        let applySqlCmdWithOrder order sqlFunc=
 
            order
            |>List.iter(fun (tableName,entity)-> 
                    (
                     //log $"{tableName}"
                     getAllEntityForSiteNameForOneTable entity
                     |>sqlFunc
                    ))

        log $"-----Apply function {sqlCmd.ToString()} {siteName}-----"

        try
            match sqlCmd with
            |Cmd.Insert -> 
                applySqlCmdWithOrder insOrder insertManyToDb
                isInsertSuccess siteName
            |Cmd.Delete -> 
                applySqlCmdWithOrder delOrder delManyFromDb
                isDelSuccess siteName
            
            Some(siteName)
        with
            | ex -> (
                log $"Error: {ex.Message}"
                saveToErrorFile siteName ex.Message
                None)
