
## Operatorar
- `a >=> b` Dersom a returnerer ein verdi, kall b med den verdien. Dersom a returnerer None, return None.
    * Kan vere nyttig dersom ein vil ha ein pipeline, altså "dersom A går bra, send det viare til B, og dersom det går bra, send det viare til C"
- `a <|> b` Dersom a returnerer ein verdi, return den verdien. Viss ikkje, kall b og returner den verdien.
    * Kan vere nyttig for å implementere "default med fallback" eller OR-logikk.

### Presedens
"Operatorane" over fungerer som vanlege funksjonskall. Uttrykket `a >=> b` er det same som `(>=>) a b`. Dermed blir 

`a >=> b >=> c`

ekvivalent med

`(>=>) ((>=>) a b) c`

