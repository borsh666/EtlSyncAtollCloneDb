namespace Etl

open FSharp.Data.Sql.Common
open FSharp.Data.Sql
open FSharp.Data
open Literals

module Models =

    type MyJsonProvider = JsonProvider<SitesWithError>

    type SiteError = {SiteName:string; Error:string}

    type Item = 
        | Antenna
        | Feeder
        | Tma

    type Cmd = 
        | Insert=1
        | Delete=2

    type Technology = 
        | GSM = 1
        | UMTS = 2
        | LTE = 3 
        | DSS = 4 
        | NR = 5 


    type Band = 
        | B7 = 700
        | B8  = 800
        | B9  = 900
        | B18 = 1800
        | B21  = 2100
        | B26 = 2600
        | B36 = 3600


    type TransParam = {
        SiteName:string
        TxId:string
        Fband:string
        AntennaName:string option
        Tilt:single option
        Azimut:single option
        Height:single option
        CfSiteProgress:string option
        CfParametersPlan:string option
    }

    type Entity = {
        EntityName:string
        Entity: SqlEntity
    }

    type AntennaParam = {
        Band:Band option;
        Pattern:string;
        Etilt: single option;
        AntennaName:string option;
        Frequency:double option
    }


    type ParamForAdd = {
        SiteName:string
        Sector:string;
        CellNameLastDigit:char;
        Fband:string;
        Tech:Technology
    } with

        member this.GetCellName  = 
            this.SiteName[2..5] + this.Sector + this.CellNameLastDigit.ToString()


    type ColumGetSet = {
        ColumnName:string;
        
    } with

        member this.Get (entity: Common.SqlEntity)   = 
            entity.GetColumn(this.ColumnName)
            
        member this.Set value (entity: Common.SqlEntity)    = 
            entity.SetColumn(this.ColumnName, value)