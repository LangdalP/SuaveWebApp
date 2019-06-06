
module WebParts

open Suave
open Suave.Utils.Collections

let random (parts: WebPart list) (x: HttpContext) =
    let rng = System.Random()  
    let chosenPart = List.item (rng.Next(List.length parts)) parts
    async {
        return! chosenPart x
    }

let private toBytes =
    function
    | Bytes b -> Some b
    | _ -> None

let private addContentInternal bytes : WebPart =
    fun context ->
        let oldContent = context.response.content |> toBytes
        let newBytes =
            match oldContent with
            | Some oldBytes -> Array.concat [oldBytes ; bytes]
            | None -> bytes
        let newContext = {
            context with response = { context.response with status = HTTP_200.status; content = Bytes newBytes }
        }
        async.Return (Some newContext)

let say text = addContentInternal (UTF8.bytes text)

let file filePath =
    let content = System.IO.File.ReadLines(filePath) |> Seq.fold (+) ""
    let contentBytes = UTF8.bytes content
    fun context ->
        let newHeaders = ("Content-Type", "text/css") :: context.response.headers
        let newContext = {
            context with response = { context.response with status = HTTP_200.status; content = Bytes contentBytes; headers = newHeaders }
        }
        async.Return (Some newContext)
