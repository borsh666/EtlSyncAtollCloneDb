namespace Etl

open FSharp.Data.LiteralProviders
open System

module Literals =
    
    [<Literal>]
    let SiteNameRef = "SF7000A"

    [<Literal>]
    let LogFilePath = $"D:\MEDIATION\AtollSimulationDbSync\Log"
  
    let logFileName = sprintf "log%s.txt" (DateTime.Now.ToString("yyyyMMdd"))
  
    //CF_SITE_TYPE
    [<Literal>]
    let SiteTypes = "Macro"

    //cf_candidate
    [<Literal>]
    let SiteCandidate = "Accepted"

    [<Literal>]
    let MaxSectors = 4

    [<Literal>]
    let DbNameOrig = "ATOLL_5GMRAT"

    [<Literal>]
    let DbNameClone = "AtollSimulation"

    [<Literal>]
    let useOptionTypes = FSharp.Data.Sql.Common.NullableColumnType.OPTION
    
    [<Literal>]
    let ConnStrATOLL_5GMRAT = TextFile<"db/connStrATOLL_5GMRAT.txt">.Text

    [<Literal>]
    let ConnStrAtoll_Simulation_DB = TextFile<"db/connStrAtollSimulation.txt">.Text
 
    [<Literal>]
    let ConnStrAchko = TextFile<"db/connStrAchko.txt">.Text

    //let ConnStrAtoll_Simulation_DB = ConnStrAchko 

    [<Literal>]
    let MailFrom = "np_team@vivacom.bg"

    [<Literal>]
    let MailFromName = "NDT"
    
    [<Literal>]
    let MailSubject = "Atoll Simulation database sync"

    [<Literal>]
    let MailBody = "" 

    let MailsTo = ["stefan.velinov@vivacom.bg"] 
 

    [<Literal>]
    let SitesWithError = "D:\MEDIATION\AtollSimulationDbSync\site_error.json"

