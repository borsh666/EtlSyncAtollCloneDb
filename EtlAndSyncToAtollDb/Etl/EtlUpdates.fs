namespace Etl

open FSharp.Data.Sql
open Db
open Models
open Dapper
open Tools
open Etl
open FSharp.Data.Sql.Common
open FileService
open Filters
open DbInsertService
open IsEtlPossible

module EtlCheckUpdates =

    let private checkNewElementAndUpdate (element:Item, entityOrig: SqlEntity seq, entityClone: SqlEntity seq) =

        log $"Checking for new {element.ToString()}...."

        let entityOrigFilter =
            entityOrig 
            |> Seq.map  colName.Get

        let entityCloneFilter = 
            entityClone 
            |> Seq.map colName.Get

        let newElements = 
            entityOrigFilter 
            |> Seq.except entityCloneFilter

        let getEntityFromOrig (eleName:string) =
            entityOrig 
            |> Seq.filter (fun entity -> colName.Get entity  = eleName)

        if Seq.isEmpty newElements then
            log $"No new {element.ToString()} for update!"
        else
            (commonMsgForNewItems newElements
             |> Seq.map getEntityFromOrig
             |> Seq.collect id
             |> Seq.iter (fun entity ->
                 (entity |> insertToDb 
                  log $"{element.ToString()} {colName.Get entity} is in db!")))


    let checkNewAntennaAndUpdate () =
        checkNewElementAndUpdate (Item.Antenna,antOrigGeneric, antCloneGeneric)


    let checkNewFeedersAndUpdate () =
        checkNewElementAndUpdate (Item.Feeder,feedOrigGeneric, feedCloneGeneric)


    let checkNewTmaAndUpdate () =
        checkNewElementAndUpdate (Item.Tma,tmaOrigGeneric, tmaCloneGeneric)


    let checkUpdateSites () =
        log "Checking for  site updates...."
       
        let sitesForUpdate = 
            getCommonParamAllTransOrigDb
            |>Seq.except getCommonParamAllTransCloneDb
            |>Seq.map (fun trans -> trans.SiteName)
            |>Seq.distinct
            |>List.ofSeq


        let sitesFilterBySiteTypeAndCandidate siteName  = 
            getTableSitesBySiteTypeAndCandidate sitesOrigGeneric 
            |>Seq.contains siteName

        sitesForUpdate
        |>List.filter sitesFilterBySiteTypeAndCandidate
        |>List.filter isSiteCapableToAdd
        |>commonMsgForNewItems 

    //let checkNewSites() = 
    //    log "Checking for  new sites...."
        
    //    let getTableSitesBySiteTypeAndCandidateOrig  = 
    //        getTableSitesBySiteTypeAndCandidate sitesOrigGeneric

    //    let getTableSitesBySiteTypeAndCandidateClone  = 
    //        getTableSitesBySiteTypeAndCandidate sitesCloneGeneric

    //    getTableSitesBySiteTypeAndCandidateOrig
    //    |>Seq.except getTableSitesBySiteTypeAndCandidateClone
    //    |>Seq.filter isSiteCapableToAdd
    //    |> commonMsgForNewItems 

