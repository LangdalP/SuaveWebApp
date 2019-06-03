// Learn more about F# at http://fsharp.org

open System
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let app =
  choose
    [ GET >=> choose
        [ path "/" >=> OK "Index pindex"
          path "/hello" >=> OK "Hello GET"
          path "/append" >=> WebParts.say "Heisann" >=> WebParts.say " sveisann!"
          pathScan "/yo/%s/%s" (fun (x, y) -> OK (sprintf "Yo %s u r %s but enough about that what u doin?" x y))
          pathScan "/yo/%s" ((sprintf "Yo %s what u doin?") >> OK) // 2. parameter til pathScan er ekvivalent med (fun a -> OK (a |> sprintf "Yo %s what u doin?"))
          path "/goodbye" >=> OK "Good bye GET" ]
      POST >=> choose
        [ path "/hello" >=> OK "Hello POST"
          path "/goodbye" >=> OK "Good bye POST" ] ]

[<EntryPoint>]
let main argv =
    startWebServer defaultConfig app
    0 // return an integer exit code
