
module WebParts

open Suave
open FSharp.Json

let random (parts: WebPart list) (x: HttpContext) =
    let rng = System.Random()  
    let chosenPart = List.item (rng.Next(List.length parts)) parts
    async {
        return! chosenPart x
    }

let private httpContentToBytes =
    function
    | Bytes b -> Some b
    | _ -> None

let private addContentInternal bytes : WebPart =
    fun context ->
        let oldContent = context.response.content |> httpContentToBytes
        let newBytes =
            match oldContent with
            | Some oldBytes -> Array.concat [oldBytes ; bytes]
            | None -> bytes
        let newContext = {
            context with response = { context.response with status = HTTP_200.status; content = Bytes newBytes }
        }
        async.Return (Some newContext)

let say text = addContentInternal (UTF8.bytes text)

let file filePath mediaType =
    let content = System.IO.File.ReadLines(filePath) |> Seq.fold (+) ""
    let contentBytes = UTF8.bytes content
    fun context ->
        let newHeaders = ("Content-Type", mediaType) :: context.response.headers
        let newContext = {
            context with response = { context.response with status = HTTP_200.status; content = Bytes contentBytes; headers = newHeaders }
        }
        async.Return (Some newContext)

let json data =
    let dataSerialized = Json.serialize data
    let contentBytes = UTF8.bytes dataSerialized
    fun context ->
        let newHeaders = ("Content-Type", "application/json") :: context.response.headers
        let newContext = {
            context with response = { context.response with status = HTTP_200.status; content = Bytes contentBytes; headers = newHeaders }
        }
        async.Return (Some newContext)

let authorize =
    let secret = "31337"
    fun context ->
        let secretHeader = context.request.headers |> List.tryFind (fun (key, value) -> key = "secret")
        let foundCorrectSecret =
            match secretHeader with
            | Some (_, secretFromHeader) -> secretFromHeader = secret
            | None -> false
        match foundCorrectSecret with
        | true -> async.Return (Some context)
        | false -> async.Return None