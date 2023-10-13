namespace Etl

open Models
open Literals
open FSharp.Data.Sql
open FSharp.Data.SqlClient.Internals
open System.Data
open Tools
open Db

module Filters = 

    //cellNameToAdd - new cellName to add from siteName
    let private filterEntityRef (cellNameRef: string) cellNameToAdd =
        getFirstFourDigits cellNameRef = siteDigitRef
        && getCellNameLastDigit cellNameRef = getCellNameLastDigit cellNameToAdd
        && getSectorFromCellName cellNameRef = getSectorFromCellName cellNameToAdd

    let filterEntityFromTxIdRef txId (entity: Common.SqlEntity) =
        filterEntityRef (colTxId.Get entity) txId

    let filterEntityFromCellIdRef txId (entity: Common.SqlEntity) =
        filterEntityRef (colCellId.Get entity) txId

    let filterBySiteTypeAndCandidate siteType siteCandidate= 
        siteType = Some(SiteTypes) && 
        siteCandidate = Some(SiteCandidate)
        
    let getTableSitesBySiteTypeAndCandidate (tableSite: Common.SqlEntity seq) =
        tableSite
        |>Seq.filter(fun entity-> 
              colSiteType.Get entity =  SiteTypes &&
              colCandidate.Get entity = SiteCandidate)
        |>Seq.map colName.Get
      

    let filterEntityBySiteName siteName (entity:Common.SqlEntity) =

        let isColExistsInEntity (col, tables)   = 
            entity.ColumnValues
            |>Seq.exists(fun (colName,_) -> 
                colName = col.ColumnName && 
                tables|>List.contains entity.Table.Name
                )

        let filterBySitedigit  (colObj:ColumGetSet) = 
            getSiteDigit siteName = getFirstFourDigits (colObj.Get entity)

        let filterBySiteName  (colObj:ColumGetSet) = 
            siteName = colObj.Get entity      


        mapColNameTableNames
        |>List.filter isColExistsInEntity 
        |>List.map(function colObj,_->
                            match colObj with
                            |colObj when colObj=colName || 
                                                   colObj=colSiteName
                                                   -> filterBySiteName(colObj) 
                            |_ -> filterBySitedigit colObj)
        |>List.exists id 

