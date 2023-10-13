namespace Etl

open Etl
open EtlCheckUpdates
open Models
open System
open FileService
open Mail
open EtlAddTech
open IsEtlPossible


module Proogram = 

    [<EntryPoint>]
    let main args = 

        log $"-----START-----{DateTime.Now}"

        let sw = System.Diagnostics.Stopwatch()
        sw.Start()

        try
            checkNewAntennaAndUpdate() 
            checkNewTmaAndUpdate()
            checkNewFeedersAndUpdate()

            let engine siteName =
               etl Cmd.Delete siteName
               |>Option.map(etl Cmd.Insert)|> Option.flatten
               |>getMissTech 
               |>List.filter isTechCapableToAdd 
               |>List.iter techToAdd 


            checkUpdateSites()
            |>List.iter engine 

            sendEmail ()

        with
        |ex  -> log($"Error - {ex.Message}")

        sw.Stop()
        Console.WriteLine($"Time elapsed: {sw.Elapsed}")
        sw.Reset()

        log $"-----END-----{DateTime.Now}"
        0

