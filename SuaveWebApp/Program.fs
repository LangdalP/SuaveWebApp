// Learn more about F# at http://fsharp.org

open System
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open Suave.DotLiquid

type Model = { text : string }

let app =
  choose
    [ GET >=> choose
        [ path "/" >=> page "index.liquid" { text = "You have reached the index page" }
          path "/test" >=> OK "You have reached the /test page"
          pathScan "/test/%s/%s" (fun (x, y) -> OK (sprintf "You have reached the test page with the path parameters %s and %s" x y))
          path "/public/css/style.css" >=> WebParts.file "./public/css/style.css" "text/css" ]
      POST >=> choose
        [ path "/test" >=> OK "You have reached the /test page with a POST" ] ]

[<EntryPoint>]
let main argv =
    setTemplatesDir "./templates"
    startWebServer defaultConfig app
    0 // return an integer exit code
