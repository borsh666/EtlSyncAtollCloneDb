namespace Etl

open Models
open System
open System.IO
open Literals
open FSharp.Data.Sql
open Newtonsoft.Json

module FileService=

    let saveToErrorFile siteName error = 

        let jsonData = 
            MyJsonProvider.Load(SitesWithError)

        let newItem = MyJsonProvider.Root(siteName,error)
       
        let updatedJsonArray = 
           jsonData|>Array.append  [| newItem |]
       
        updatedJsonArray
        |>Array.map (fun x->  
            {SiteName =x.SiteName;Error =x.Error})
        |> JsonConvert.SerializeObject 
        |> function error -> 
                     File.WriteAllText (SitesWithError,error)

    let isSiteNameInFileSiteError siteName =
        MyJsonProvider.Load(SitesWithError)
        |>Array.exists(fun x-> 
            x.SiteName = siteName)

    let log message =
        try
            let path = Path.Combine(LogFilePath, logFileName)
            File.AppendAllText(path, message + "\n")
        with ex ->
            printfn "Error writing to file: %s" ex.Message

        printfn "%s" message