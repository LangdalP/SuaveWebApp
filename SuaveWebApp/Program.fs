// Learn more about F# at http://fsharp.org

open System
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open Suave.DotLiquid
open DotLiquid
open Suave.Files

type Model = { text : string }

let app =
  choose
    [ GET >=> choose
        [ path "/" >=> OK "Index pindex"
          path "/public/css/style.css" >=> WebParts.file "./public/css/style.css" "text/css"
          path "/hello" >=> OK "Hello GET"
          pathScan "/template/%s" (fun x -> page "index.liquid" { text = "Hello, " + x })
          path "/append" >=> WebParts.say "Heisann" >=> WebParts.say " sveisann!"
          pathScan "/yo/%s/%s" (fun (x, y) -> OK (sprintf "Yo %s u r %s but enough about that what u doin?" x y))
          pathScan "/yo/%s" ((sprintf "Yo %s what u doin?") >> OK) // 2. parameter til pathScan er ekvivalent med (fun a -> OK (a |> sprintf "Yo %s what u doin?"))
          path "/goodbye" >=> OK "Good bye GET" ]
      POST >=> choose
        [ path "/hello" >=> OK "Hello POST"
          path "/goodbye" >=> OK "Good bye POST" ] ]

[<EntryPoint>]
let main argv =
    setTemplatesDir "./templates"
    startWebServer defaultConfig app
    0 // return an integer exit code
