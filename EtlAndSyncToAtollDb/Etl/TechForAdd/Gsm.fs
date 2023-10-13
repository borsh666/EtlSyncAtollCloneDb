namespace Etl

open Models
open FSharp.Data.Sql
open Db
open Etl
open Tools
open Filters

module Gsm =

    let gsmToAdd (transParam: TransParam) =

        //Add Gtransmitters
        let gtransForAdd =
            gtransCloneGeneric
            |> Seq.find (filterEntityFromTxIdRef transParam.TxId)
            |> function
                 entity ->
                    (let cloneEntity = entity.Clone()
                     let cellId = getCellIdFromCellName transParam.TxId
                     colCellIdentity.Set cellId cloneEntity
                     { EntityName = "Gtransmitters"
                       Entity = mapTransParamToSqlEntity (transParam, cloneEntity) })


        //Add Gtrxs
        let gtrxsForAdd =
            gtrxsCloneGeneric
            |> Seq.find (filterEntityFromTxIdRef transParam.TxId)
            |> function
                 entity ->
                    (let cloneEntity = entity.Clone()
                     colTxId.Set transParam.TxId cloneEntity
                     { EntityName = "Gtrxs"
                       Entity = cloneEntity })

        //Add Gtrgs
        let gtrgsForAdd =
            gtrgsCloneGeneric
            |> Seq.filter  (filterEntityFromTxIdRef transParam.TxId)
            |> Seq.map (fun entity ->
                (let cloneEntity = entity.Clone()
                 colTxId.Set transParam.TxId cloneEntity
                 { EntityName = "Gtrgs"
                   Entity = cloneEntity }))


        Seq.append
            (seq {
                gtransForAdd
                gtrxsForAdd
            })
            gtrgsForAdd
