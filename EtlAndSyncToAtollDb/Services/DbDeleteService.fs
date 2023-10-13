namespace Etl

open FSharp.Data.Sql
open Db
open Tools
open FileService

module DbDeleteService =

    //let delFromDb  (obj:Common.SqlEntity)  = 
    //    log($"Try to delete row in db - {DbNameClone}....")

    //    let id = obj.ColumnValues 
    //             |> Seq.find(fun (colName,_) -> colName = "DB_RECORD_ID")
    //             |> function (_,colValue) -> colValue |> unbox<int64>

    //    let del = sprintf "DELETE FROM %s WHERE DB_RECORD_ID=%i" obj.Table.FullName id
        
    //    executeSql del

    let delManyFromDb  (entities:Common.SqlEntity list)  =       
        if not (entities|>List.isEmpty) then
            //log($"Try to delete row in db - {DbNameClone} table {getTableName entities}....")
        
            let getId (obj:Common.SqlEntity) = 
               obj.ColumnValues 
               |> Seq.find(fun (colName,_) -> colName = "DB_RECORD_ID")
               |> function (_,colValue) -> colValue |> unbox<int64>         

            let ids = entities
                      |> List.map getId

            let del = sprintf $"DELETE FROM {getTableName entities} 
                                WHERE DB_RECORD_ID IN ({sequenceToString ids})" 
        
            executeSql del
        
    
    let isDelSuccess siteName =
        dbClone.Sites
        |>Seq.exists(fun entity-> 
                entity.Name = siteName)
        |>function entity-> 
                   if entity then log $"Delete unsuccessful - {siteName}"
                   else log $"Delete Ok - {siteName}"
            

    let delOrder = 
        [
            "Xgcells5gnr", xgcells5gnrCloneGeneric;
            "Xgcellslte", xgcellslteCloneGeneric;
            "Gxgneighbours"  ,gxgneighboursCloneGeneric;      
            "Xguneighbours"  ,xguneighboursCloneGeneric;     
            "Xggneighbours"  ,xggneighboursCloneGeneric;     
            "Xgneighbours"  ,xgneighboursCloneGeneric;    
            "Xgcellsuids", xgcellsuidsCloneGeneric;
            "Xgsecondaryantennas", xgsecondaryantennasCloneGeneric;
            "Usecondaryantennas", usecondaryantennasCloneGeneric
            "Gsecondaryantennas", gsecondaryantennasCloneGeneric;
            "Xgrepeaters",xgrepeatersCloneGeneric
            "Urepeaters",urepeatersCloneGeneric
            "Xgtransmitters", xgtransCloneGeneric;
            "Uneighbours", uneighboursCloneGeneric;
            "Guneighbours", guneighboursCloneGeneric;
            "Upscforbidpairs", upscforbidpairsCloneGeneric;
            "Gneighconstraints", gneighconstraintsCloneGeneric;
            "Ugneighbours"  ,ugneighboursCloneGeneric;  
            "Ucells", ucellsCloneGeneric;
            "Utransmitters", utransCloneGeneric;
            "Gtrxs", gtrxsCloneGeneric;
            "Gtrgs", gtrgsCloneGeneric;
            "Gneighbours"  ,gneighboursCloneGeneric;    
            "Grepeaters"  ,grepeatersCloneGeneric;    
            "Gtxslists"  ,gtxslistsCloneGeneric;    
            "Gtransmitters", gtransCloneGeneric;
            "Siteslists", siteslistsCloneGeneric;
            "Sites", sitesCloneGeneric
        ]