module Adacola.ShukaShoot.Police

open System.Text.RegularExpressions

let isSuspendText text =
    Regex.IsMatch(text, @"(落ち(る|ります|よう?)|電源切(る|ります|ろう?)|寝(る|ます|よう?))$")

let (|SuspendTweet|_|) = function
    | Twitter.MyTweet(m) ->
        match m with
        | Twitter.Retweet(_) -> None
        | _  when isSuspendText m.Status.Text -> Some(m)
        | _ -> None
    | _ -> None

let (|StartTweet|_|) = function
    | Twitter.MyTweet(m) -> Some(m)
    | _ -> None
