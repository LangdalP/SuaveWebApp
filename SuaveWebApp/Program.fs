// Learn more about F# at http://fsharp.org

open System
open Suave

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    startWebServer defaultConfig (Successful.OK "Hello World!")
    0 // return an integer exit code
