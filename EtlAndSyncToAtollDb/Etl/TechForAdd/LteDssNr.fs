namespace Etl

open Models
open FSharp.Data.Sql
open Db
open Tools
open Filters


module LteDssNr =


    let lteDssNrToAdd (transParam: TransParam) =
        //Add Xgtransmitters
        let xgtransForAdd =
            xgtransCloneGeneric
            |> Seq.find (filterEntityFromTxIdRef transParam.TxId)
            |> function
                 entity ->
                    { EntityName = "Xgtransmitters"
                      Entity = mapTransParamToSqlEntity (transParam, entity.Clone()) }

        //Add Xgcellsuids
        let xgcellsuidsForAdd =
            xgcellsuidsCloneGeneric
            |> Seq.find (filterEntityFromCellIdRef transParam.TxId)
            |> function
                 entity ->
                    (let cloneEntity = entity.Clone()
                     colCellId.Set transParam.TxId cloneEntity
                     colTxId.Set transParam.TxId cloneEntity

                     { EntityName = "Xgcellsuids"
                       Entity = cloneEntity })

        let siteDigitOrig = getSiteDigit transParam.SiteName

        let setColCellIdAndUniqueId entityName (entity: Common.SqlEntity) =
            colCellId.Set transParam.TxId entity
            let uniqueId =  (colUniqueId.Get entity).ToString()
                                   .Replace(siteDigitRef,siteDigitOrig)
            //let uniqueId = replaceSiteDigitRefWithOrig ((colUniqueId.Get entity), siteDigitOrig)
            colUniqueId.Set uniqueId entity

            { EntityName = entityName
              Entity = entity }


        //Add Xgcellslte
        let xgcellsForAddOption =
            xgcellslteCloneGeneric
            |> Seq.tryFind (filterEntityFromCellIdRef transParam.TxId)
            |> Option.map (fun entity-> 
                          setColCellIdAndUniqueId "Xgcellslte" (entity.Clone()))

        let entities =
            match xgcellsForAddOption with
            | Some xgcellsForAdd ->
                seq {
                    xgtransForAdd
                    xgcellsuidsForAdd
                    xgcellsForAdd
                }
            | None ->
                seq {
                    xgtransForAdd
                    xgcellsuidsForAdd
                }

        //Add Xgcells5gnr
        let xgcells5gnrForAdd () =
            xgcells5gnrCloneGeneric
            |> Seq.find (filterEntityFromCellIdRef transParam.TxId)
            |> function entity -> entity.Clone()
            |> setColCellIdAndUniqueId "Xgcells5gnr"

        let isCellNameDssOrNr  =
            let lastChar = 
                Set.ofList [ 'N'; 'F' ]
            Set.intersectMany [ transParam.TxId |> Set.ofSeq; lastChar ] 
            |> Set.isEmpty |> not

        if isCellNameDssOrNr  then
            [ xgcells5gnrForAdd () ] |> Seq.append entities
        else
            entities
