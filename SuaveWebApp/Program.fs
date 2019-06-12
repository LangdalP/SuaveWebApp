// Learn more about F# at http://fsharp.org

open System
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open Suave.DotLiquid

type Model = { text : string }

type JsonModel1 = {
  number : int;
  numbers : int list
}

type JsonModel2 = {
  text : string;
  texts : string list
}

let app =
  choose
    [ GET >=> choose
        [ path "/" >=> page "index.liquid" { text = "You have reached the index page" }
          path "/test" >=> WebParts.say "You have reached" >=> WebParts.say " the /test page"
          path "/test-query" >=> request (fun r -> OK (sprintf "%A" r.query))
          path "/json1" >=> WebParts.json { text = "foo"; texts = ["foo"; "bar"]}
          path "/json2" >=> WebParts.json { number = 1; numbers = [1; 3; 3; 7]}
          path "/secret" >=> WebParts.authorize >=> OK "You may enter"
          pathScan "/test/%s/%s" (fun (x, y) -> OK (sprintf "You have reached the /test page with the path parameters %s and %s" x y))
          path "/public/css/style.css" >=> WebParts.file "./public/css/style.css" "text/css" ]
      POST >=> choose
        [ path "/test" >=> OK "You have reached the /test page with a POST" ] ]

[<EntryPoint>]
let main argv =
    setTemplatesDir "./templates"
    startWebServer defaultConfig app
    0 // return an integer exit code
