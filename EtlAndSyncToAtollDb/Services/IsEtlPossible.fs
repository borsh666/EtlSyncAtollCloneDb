namespace Etl

open FSharp.Data.Sql
open Db
open Models
open Tools
open Literals
open FileService
open System

module IsEtlPossible = 

    let isTechCapableToAdd (paramForAdd : ParamForAdd) =

        let isSectorOk = paramForAdd.Sector |> String.forall Char.IsDigit  &&
                         Int32.Parse(paramForAdd.Sector) <= MaxSectors
        
        let isCellNameOk =
            match paramForAdd.CellNameLastDigit with
            |_ when Char.IsDigit paramForAdd.CellNameLastDigit ||
                    paramForAdd.CellNameLastDigit = 'F' ||
                    paramForAdd.CellNameLastDigit = 'N'-> true 
            |_ -> false

       
        let logAndSaveToSiteErrorJson = 
            logAndSaveToSiteErrorJson paramForAdd.SiteName
        
        match () with
        | () when not isSectorOk -> (
            $"Error: Sector is not digit/missing ot number>{MaxSectors}!"
            |>logAndSaveToSiteErrorJson
            false)
        | () when not isCellNameOk -> (
            $"Error:CellName last symbol is not digit or not contain chars F or N!" 
            |>logAndSaveToSiteErrorJson
            false)
        |_ -> (
            //log $"Site:{paramForAdd.SiteName}, sector:{paramForAdd.Sector}-new tech {paramForAdd.Tech} found to add for band {fbandToBand paramForAdd.Fband}"
            true)

    let isSiteCellNameFirst4digitNotSame siteName = 
        getCommonParamAllTransOrigDb
        |>Seq.filter(fun x-> 
            x.SiteName = siteName &&
            not (getSiteDigit x.SiteName = getFirstFourDigits x.TxId))
        |>List.ofSeq
        |>function 
               |[]->false
               |_->true

    let isSiteCapableToAdd (siteName:string) =
        match () with
        | () when (isSiteCellNameFirst4digitNotSame siteName) -> (
            $"Error: Site {siteName} 4digit are not equal to first 4digit of txId in transTables!"
            |>logAndSaveToSiteErrorJson siteName
            false)
        | () when  isSiteNameInFileSiteError siteName -> (
            log $"Error: {siteName} is in file SiteError.json"
            false) 
        | () when not (siteName[2..5]|> Seq.forall Char.IsDigit) -> (
            $"Error: Site {siteName} doesn't have four digit!"
            |>logAndSaveToSiteErrorJson siteName
            false)
        |_ -> true

    let isAntCapable antPat bandToAdd =
        let getAllAntennas() = 
            query {
                    for ant in dbOrig.Antennas do
                    select
                        ({
                             Band = antFreqToAntBand ant.Frequency
                             Pattern = ant.Name
                             Etilt = ant.ElectricalTilt
                             AntennaName = ant.PhysicalAntenna
                             Frequency = ant.Frequency
                         })
            }
        
        match bandToAdd with
        |band when band=Band.B7 || band=Band.B8 -> Seq.empty
        |_ -> (
            let allAnt = getAllAntennas()
            let antParam = 
                allAnt
                |> Seq.find(fun x-> x.Pattern = antPat)
            
            allAnt
            |> Seq.filter(fun x-> antParam.AntennaName = x.AntennaName &&
                                  Option.isSome x.Band &&
                                  bandToAdd = x.Band.Value &&
                                  antParam.Etilt = x.Etilt)
        )


