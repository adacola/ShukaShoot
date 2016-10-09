﻿module Adacola.ShukaShoot.Police

open System
open System.Text.RegularExpressions

let private random = Random()
let private waitIntervals = [|None; Some 1; Some 3; Some 8; Some 13; Some 21; Some 34; Some 55; Some 89; Some 144|]

let activeTime = TimeSpan.FromMinutes(60.0)

let getFavoriteWaitSecond() =
    waitIntervals.[random.Next waitIntervals.Length]

let favorite tokens statusID =
    async {
        match getFavoriteWaitSecond() with
        | Some second ->
            do Console.WriteLine("{0} 秒待機", second)
            do! Async.Sleep (second * 1000)
            let! status = Twitter.favoriteTweet tokens statusID
            return Some(status)
        | None ->
            return None
    }

let isFavoriteText text =
    Regex.IsMatch(text, @"((し|シ)\s*(ゅ|ゆ|ュ|ユ)\s*(か|カ|力)\s*((し|シ)\s*(ゅ|ゆ|ュ|ユ)\s*(ー|う|ぅ|ウ|ゥ)|(ぴ|ピ))|朱夏)(?!警察)")

let isSuspendText text =
    Regex.IsMatch(text, @"(落ち(る|ります|よう?)|電源切(る|ります|ろう?)|寝(る|ます|よう?))$")

let (|FavoriteTweet|_|) = function
    | Twitter.MyTweet(_) -> None
    | Twitter.Tweet(m) -> 
        match m with
        | Twitter.Retweet(_) -> None
        | _ when isFavoriteText m.Status.Text -> Some(m, m.Status.Id)
        | _ -> None
    | _ -> None

let (|SuspendTweet|_|) = function
    | Twitter.MyTweet(m) when isSuspendText m.Status.Text -> Some(m)
    | _ -> None