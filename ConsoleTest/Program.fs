
open Etl
open Models
open EtlAddTech
open Db



"SF1042E"
|>Etl.etl Cmd.Delete
|>Option.map (Etl.etl Cmd.Insert)///|> Option.flatten|> List.collect getMissTech    
//|> List.iter techToAdd


printfn "Hello from F#"
