
module WebParts

open Suave

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