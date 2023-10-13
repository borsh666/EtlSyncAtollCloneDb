namespace Etl

open FSharp.Data.Sql
open System
open Db
open Tools
open Literals
open FileService


module DbInsertService =

    let getColNamesAndColValues (obj:Common.SqlEntity) =

        let filterByColName (colName,_) = 
            not (Seq.contains colName ["DB_RECORD_ID";"HISTORY_ID";"MODIFIED_DATE"])
        let filterByColValue (_,colValue) = colValue <> null 

        let filter = 
            obj.ColumnValues 
            |> Seq.filter filterByColName
            |> Seq.filter filterByColValue

        let columnNames = filter |> Seq.map (fun (colName,_) -> colName)
        let columnValues = filter |> Seq.map (fun (_,colValue) -> (
            match colValue with
            | :? string -> box (
                                let clearColValue = 
                                    colValue.ToString()
                                   //         .Replace(",", " ")
                                            .Replace("'", "")
                                sprintf "'%A'" clearColValue
                                ) 
            | :? (byte seq) -> box (sprintf "%A" (objOfBytesToString colValue))
            | _ -> colValue
            ))

        let columns = String.Join(", ", columnNames)
        let values = String.Join(", ", columnValues)
        (columns,values)
        
    let insertSqlStatement tableName (colNames, colValues)  = 
         (sprintf $"INSERT INTO {tableName} ({colNames}) 
                               VALUES {colValues}" )
                      .Replace("\"", "")
                      .Replace("True","1")
                      .Replace("False","0")

    let insertToDb  (obj:Common.SqlEntity)  =

        getColNamesAndColValues obj
        |>function (colNames,colValues) -> (colNames,"(" + colValues + ")")
        |>insertSqlStatement obj.Table.FullName
        |>executeSql  
        |>ignore
       


    let insertManyToDb  (entities:Common.SqlEntity list)  =
       // log($"Try to insert rows in db - {DbNameClone}....")
        entities 
        |>List.iter(fun entity-> insertToDb entity )
          
    let isInsertSuccess siteName =
        dbClone.Xgcellslte
        |>Seq.exists(fun entity-> 
               getFirstFourDigits entity.CellId = getSiteDigit  siteName)
        |>function entity-> 
                   if entity then log $"Insert unsuccessful - {siteName}"
                   else log $"Insert Ok - {siteName}"

    let insOrder = 
        [
            "Sites", sitesOrigGeneric
            "Gtransmitters", gtransOrigGeneric;
            "Gtrxs", gtrxsOrigGeneric;
            "Gtrgs", gtrgsOrigGeneric;
            "Utransmitters", utransOrigGeneric;
            "Ucells", ucellsOrigGeneric;
            "Xgtransmitters", xgtransOrigGeneric;
            "Xgcellsuids", xgcellsuidsOrigGeneric;
            "Xgcellslte", xgcellslteOrigGeneric;
            "Xgcells5gnr", xgcells5gnrOrigGeneric;
        ]
