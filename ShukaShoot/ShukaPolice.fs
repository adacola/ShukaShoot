module Adacola.ShukaShoot.ShukaPolice

open System
open System.Text.RegularExpressions
open MathNet.Numerics.Random

let private random = MersenneTwister(true)
let private shukaUserID = 215805231L

let activeTime = TimeSpan.FromMinutes(60.0)

let getFavoriteWaitSecond ignoreRatio (waitIntervals : int[]) =
    let ignoreValue = random.NextDouble()
    if ignoreValue < ignoreRatio then None else
        waitIntervals.[random.Next waitIntervals.Length] |> Some

let favorite tokens statusID =
    async {
        let ignoreRatio = Model.config.ShukasyuuPolice.IgnoreRatio |> float
        let waitIntervals = Model.config.ShukasyuuPolice.WaitSeconds
        match getFavoriteWaitSecond ignoreRatio waitIntervals with
        | Some second ->
            do Console.WriteLine("{0} 秒待機", second)
            do! Async.Sleep (second * 1000)
            let! status = Twitter.favoriteTweet tokens statusID
            return Some(status)
        | None ->
            return None
    }

let isFavoriteText text =
    Regex.IsMatch(text, @"((し|シ)\s*(ゅ|ゆ|ュ|ユ)\s*(か|カ|力)\s*((し|シ)\s*(ゅ|ゆ|ュ|ユ)\s*(ー|～|う|ぅ|ウ|ゥ|l)|(ぴ|ピ))|(?<!#)朱夏)(?!警察)") && not (Regex.IsMatch(text, @"(\s|\A)@Saito_Shuka(\s|\Z)"))

let (|Tweet|_|) = function
    | Twitter.MyTweet(_) -> None
    | Twitter.Tweet(m) -> 
        match m with
        | Twitter.Retweet(_) -> None
        | _ when isFavoriteText m.Status.Text && not (Twitter.isReplyTo shukaUserID m) -> Some(m, m.Status.Id)
        | _ -> None
    | _ -> None
