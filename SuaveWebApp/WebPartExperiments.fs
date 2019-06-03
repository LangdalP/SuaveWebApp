
module ObsoleteWebParts

open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

let WastefulWebPart (x:HttpContext) =
    async {
        do! Async.Sleep 5000
        return! OK "I slept in!" x
    }

let NoOpWebPart (x:HttpContext) =
    async {
        return Some x
    }

let FoobieWebPart (x:HttpContext) =
    async {
        let retur = OK "Foobie" x
        return! retur
    }

let DoobieWebPart (x:HttpContext) =
    async {
        let retur = OK "Doobie" x
        return! retur
    }

// Ein WebPart som returnerer None gjer slik at neste WebPart ikkje blir køyrt ved bruk av >=>
// Dette gir ERR_NO_RESPONSE i nettlesaren
// Men <|> fungerer bra
// TODO: Finn ut når det er ønskeleg å bruke >=> framfor <|>
let NoneWebPart (x:HttpContext) =
    async {
        return None
    }

let NoneWebPart2 : WebPart =
    fun ctx ->
        let newCtx = { ctx with response = { ctx.response with status = HTTP_404.status; content = Bytes (UTF8.bytes "Feilkode: 404") }}
        async.Return (Some newCtx)

(*
CustomOk er identisk med impl. av OK i Suave
Returtypen til customOk seier at den skal returnere ein WebPart (altså HttpContext -> Async<HttpContext option>)
Eg antek at typesystemet dermed gjer dei følgande antakingane:
1. foo må vere ein HttpContext
2. ctx må vere ein HttpContext (sidan foo er ein modifisert versjon av ctx)
*)

let customOk s : WebPart =
    fun ctx ->
        let foo = { ctx with response = { ctx.response with status = HTTP_200.status; content = Bytes s }}
        async.Return (Some foo)
let CustomOk tekst = customOk (UTF8.bytes tekst)

let makeApp = 
  choose
    [ GET >=> choose
        [ path "/hello" >=> OK "Hello GET"
          path "/sleep" >=> WastefulWebPart >=> WastefulWebPart >=> WastefulWebPart (* Denne brukar 15 sekund *)
          path "/pipe1" >=> OK "1" >=> OK "2" >=> OK "3"
          path "/pipe2" >=> 
            ((>=>)
              ((>=>)
                (OK "1")
                (OK "2"))
              (OK "3"))
          path "/chain" >=> (OK "1" <|> NoneWebPart <|> OK "3") // Undersøke operator-presedens her. Utan parantes så virkar det som at (((path >=> OK) <|> NoneWebPart) <|> OK)
          path "/ok" >=> CustomOk "Heisann bytes"
          path "/random" >=> WebParts.random [ OK "Rng: 1" ; OK "Rng: 3" ; OK "Rng: 6" ; OK "Rng: 9" ]
          path "/goodbye" >=> OK "Good bye GET" ]
      POST >=> choose
        [ path "/hello" >=> OK "Hello POST"
          path "/goodbye" >=> OK "Good bye POST" ] ]