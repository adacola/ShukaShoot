module Adacola.ShukaShoot.Main

open System
open Twitter
open Police
open System.Threading
open Argu

type Arguments =
    | [<AltCommandLine("-m")>] Active_Minute of int
    | Debug
with
    interface IArgParserTemplate with
        member x.Usage =
            match x with
            | Active_Minute(_) -> sprintf "活動時間を分で指定。省略時は%d分" Police.activeTime.Minutes
            | Debug -> "デバッグモード。出力されるメッセージが増える"

[<EntryPoint>]
let main args =
    let parser = ArgumentParser.Create<Arguments>("ShukaShoot")
    let parseResult = parser.Parse(args)
    let activeMinute = parseResult.TryGetResult <@ Active_Minute @>
    let isDebug = parseResult.Contains <@ Debug @>
    let activeTime = activeMinute |> Option.map (float >> TimeSpan.FromMinutes) |> defaultArg <| Police.activeTime

    let tokens = authenticate()
    let connection = getTimelineConnection tokens
    
    let myTweetTime = ref None
    connection |> Observable.subscribe (function
        | SuspendTweet(m) ->
            Console.WriteLine("俺のツイート ({0}), 活動休止 : {1}\n", toLocal m, m.Status.Text)
            Volatile.Write(myTweetTime, None)
        | MyTweet(m) ->
            Console.WriteLine("俺のツイート ({0}), {1} まで活動 : {2}\n", toLocal m, (m.Timestamp + activeTime).LocalDateTime, m.Status.Text)
            Volatile.Write(myTweetTime , Some(m.Timestamp))
        | FavoriteTweet(m, statusID) -> 
            let maybeTimeSpan = myTweetTime |> Volatile.Read |> Option.map (fun t -> m.Timestamp - t)
            Console.WriteLine("timespan : {0}", maybeTimeSpan)
            if maybeTimeSpan |> Option.exists (fun ts -> ts < activeTime) then
                async {
                    let! status = favorite tokens statusID
                    let favoriteStr = if Option.isSome status then "しました" else "スルー"
                    do Console.WriteLine("！ ふぁぼ{0}\n@{1} ({2})\n{3}\n", favoriteStr, m.Status.User.ScreenName, toLocal m, m.Status.Text)
                } |> Async.Start
            else
                Console.WriteLine("対象だけど時間外\n@{0} ({1})\n{2}\n", m.Status.User.ScreenName, toLocal m, m.Status.Text)
        | Tweet(m) ->
            if isDebug then Console.WriteLine("対象外\n@{0} ({1})\n{2}\n", m.Status.User.ScreenName, toLocal m, m.Status.Text)
        | _ -> ())
    |> ignore
    use usingConnection = connection.Connect()
    System.Console.ReadLine() |> ignore
    0
