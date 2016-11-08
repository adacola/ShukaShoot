module Adacola.ShukaShoot.CoffeePolice

open System.Text.RegularExpressions
open System

let activeTime = Model.config.CoffeePolice.ActiveMinutes |> float |> TimeSpan.FromMinutes

let isStrictCoffee text =
    Regex.IsMatch(text, @"\Aコーヒーこぼした\Z")

let isCoffee text =
    Regex.IsMatch(text, @"コーヒー.*(こぼし|零し|溢し)(ちゃっ|てしまっ|ちまっ)?(た|て)") && not (Regex.IsMatch(text, @"(\s|\A)@(anju_inami|Rikako_Aida|suwananaka|box_komiyaarisa|Saito_Shuka|Aikyan_|Kanako_tktk|aina_suzuki723|furihata_ai|adacola)(\s|\Z)", RegexOptions.IgnoreCase))

let (|Tweet|_|) = function
    | Twitter.Tweet(m) when isCoffee m.Status.Text -> Some(m, m.Status.Id)
    | _ -> None

let (|StrictTweet|_|) = function
    | Twitter.Tweet(m) -> 
        match m with
        | Twitter.Retweet(_) -> None
        | _ when isStrictCoffee m.Status.Text -> Some(m, m.Status.Id)
        | _ -> None
    | _ -> None
