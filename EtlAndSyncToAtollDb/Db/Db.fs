namespace Etl

open FSharp.Data.Sql
open System.Data.SqlClient
open Dapper
open Literals

module Db = 
       
  
    type DbClone = SqlDataProvider<
                      ConnectionString=ConnStrAtoll_Simulation_DB, 
                      DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER,
                      UseOptionTypes=useOptionTypes
                                   >
    type DbOrig = SqlDataProvider<
                       ConnectionString=ConnStrATOLL_5GMRAT, 
                       DatabaseVendor=Common.DatabaseProviderTypes.MSSQLSERVER,
                       UseOptionTypes=useOptionTypes
                                  >
    
   
    let dbOrig = DbOrig.GetDataContext().Dbo
    let dbClone = DbClone.GetDataContext().Dbo

    let executeSql (sql:string) = 
        try
            let conn  = new SqlConnection(ConnStrAtoll_Simulation_DB)
            conn.Execute(sql) |> ignore
        with
            | :? SqlException as ex -> raise ex
    
    let sitesOrigGeneric = (dbOrig.Sites |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let gtransOrigGeneric = (dbOrig.Gtransmitters |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let gtrxsOrigGeneric = (dbOrig.Gtrxs |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let gtrgsOrigGeneric = (dbOrig.Gtrgs |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let utransOrigGeneric = (dbOrig.Utransmitters |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let ucellsOrigGeneric = (dbOrig.Ucells |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xgtransOrigGeneric = (dbOrig.Xgtransmitters  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xgcellsuidsOrigGeneric = (dbOrig.Xgcellsuids  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xgcellslteOrigGeneric = (dbOrig.Xgcellslte  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xgcells5gnrOrigGeneric = (dbOrig.Xgcells5gnr  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let antOrigGeneric = dbOrig.Antennas |> Seq.map (fun x -> x.Clone()) |> List.ofSeq
    let feedOrigGeneric = dbOrig.Feederequipments |> Seq.map (fun x -> x.Clone())
    let tmaOrigGeneric = dbOrig.Tmaequipments |> Seq.map (fun x -> x.Clone())

    
    let sitesCloneGeneric = (dbClone.Sites |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let gtransCloneGeneric = (dbClone.Gtransmitters |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let gtrxsCloneGeneric = (dbClone.Gtrxs |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let gtrgsCloneGeneric = (dbClone.Gtrgs |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let utransCloneGeneric = (dbClone.Utransmitters |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let ucellsCloneGeneric = (dbClone.Ucells |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
  
    let xgtransCloneGeneric = (dbClone.Xgtransmitters  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xgcellsuidsCloneGeneric = (dbClone.Xgcellsuids  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xgcellslteCloneGeneric = (dbClone.Xgcellslte  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xgcells5gnrCloneGeneric = (dbClone.Xgcells5gnr  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq

    let gxgneighboursCloneGeneric = (dbClone.Gxgneighbours  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xggneighboursCloneGeneric = (dbClone.Xggneighbours  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xguneighboursCloneGeneric = (dbClone.Xguneighbours  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xgneighboursCloneGeneric = (dbClone.Xgneighbours  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let uneighboursCloneGeneric = (dbClone.Uneighbours  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let ugneighboursCloneGeneric = (dbClone.Ugneighbours  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let gneighboursCloneGeneric = (dbClone.Gneighbours  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let guneighboursCloneGeneric = (dbClone.Guneighbours  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq

    let grepeatersCloneGeneric = (dbClone.Grepeaters  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let gneighconstraintsCloneGeneric = (dbClone.Gneighconstraints  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let upscforbidpairsCloneGeneric = (dbClone.Upscforbidpairs  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let gtxslistsCloneGeneric = (dbClone.Gtxslists  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let siteslistsCloneGeneric = (dbClone.Siteslists  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq    
    let xgsecondaryantennasCloneGeneric = (dbClone.Xgsecondaryantennas  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq    
    let gsecondaryantennasCloneGeneric = (dbClone.Gsecondaryantennas  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let xgrepeatersCloneGeneric = (dbClone.Xgrepeaters  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let urepeatersCloneGeneric = (dbClone.Urepeaters  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq
    let usecondaryantennasCloneGeneric = (dbClone.Usecondaryantennas  |> Seq.map(fun x-> x.Clone()))|>List.ofSeq

    let antCloneGeneric = dbClone.Antennas |> Seq.map (fun x -> x.Clone()) |> List.ofSeq
    let feedCloneGeneric = dbClone.Feederequipments |> Seq.map (fun x -> x.Clone())
    let tmaCloneGeneric = dbClone.Tmaequipments |> Seq.map (fun x -> x.Clone())

    


