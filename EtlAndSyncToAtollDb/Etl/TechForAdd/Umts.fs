namespace Etl

open Models
open FSharp.Data.Sql
open Db
open Etl
open Tools
open Filters


module Umts =

    let umtsToAdd (transParam: TransParam) =
 
        //Add Utransmitters
        let utransForAdd =
            utransCloneGeneric
            |> Seq.find (filterEntityFromTxIdRef transParam.TxId)
            |> function
                | entity ->
                    (let cloneEntity = entity.Clone()
                     colCfNodebId.Set transParam.SiteName cloneEntity

                     { EntityName = "Utransmitters"
                       Entity = mapTransParamToSqlEntity (transParam, cloneEntity) })

        //Add Ucells
        let ucellsForAdd =
            ucellsCloneGeneric
            |> Seq.find (filterEntityFromTxIdRef transParam.TxId)
            |> function
                | entity ->
                    (let cloneEntity = entity.Clone()
                     colTxId.Set transParam.TxId cloneEntity
                     colCellId.Set transParam.TxId cloneEntity
                     let cellId = getCellIdFromCellName transParam.TxId
                     colCellIdentity.Set cellId cloneEntity

                     { EntityName = "Ucells"
                       Entity = cloneEntity })

        seq {
            utransForAdd
            ucellsForAdd
        }
