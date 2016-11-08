module Adacola.ShukaShoot.Main

open System
open Twitter
open System.Threading
open Argu
open CoreTweet.Streaming

type Arguments =
    | Debug
with
    interface IArgParserTemplate with
        member x.Usage =
            match x with
            | Debug -> "デバッグモード。出力されるメッセージが増える"

[<EntryPoint>]
let main args =
    let parser = ArgumentParser.Create<Arguments>("ShukaShoot", errorHandler = ProcessExiter())
    let parseResult = parser.Parse(args)
    let isDebug = parseResult.Contains <@ Debug @>

    let tokens = authenticate()
    let connection = getTimelineConnection tokens

    let myTweetTime = ref None
    let myTweetCoffeeTime = ref None
    let isInTime activeTime (maybeTime : DateTimeOffset option ref) (dt : DateTimeOffset) =
        let maybeTimeSpan = maybeTime |> Volatile.Read |> Option.map (fun t -> dt - t)
        Console.WriteLine("timespan : {0}", maybeTimeSpan)
        maybeTimeSpan |> Option.exists (fun ts -> ts < activeTime)

    let activateShukaPolice() =
        Console.WriteLine("しゅかしゅー警察起動")
        connection |> Observable.subscribe (function
            | Police.SuspendTweet(m) ->
                Console.WriteLine("活動休止ツイート ({0}) : {1}\n", toLocal m, m.Status.Text)
                Volatile.Write(myTweetTime, None)
            | Police.StartTweet(m) ->
                Console.WriteLine("活動開始ツイート ({0}), {1} まで活動 : {2}\n", toLocal m, (m.Timestamp + ShukaPolice.activeTime).LocalDateTime, m.Status.Text)
                Volatile.Write(myTweetTime , Some(m.Timestamp))
            | ShukaPolice.Tweet(m, statusID) -> 
                if isInTime ShukaPolice.activeTime myTweetTime m.Timestamp then
                    async {
                        let! status = ShukaPolice.favorite tokens statusID
                        let favoriteStr = if Option.isSome status then "しました" else "スルー"
                        do Console.WriteLine("！ ふぁぼ{0}\n@{1} ({2})\n{3}\n", favoriteStr, m.Status.User.ScreenName, toLocal m, m.Status.Text)
                    } |> Async.Start
                else
                    Console.WriteLine("対象だけど時間外\n@{0} ({1})\n{2}\n", m.Status.User.ScreenName, toLocal m, m.Status.Text)
            | Tweet(m) ->
                if isDebug then Console.WriteLine("対象外\n@{0} ({1})\n{2}\n", m.Status.User.ScreenName, toLocal m, m.Status.Text)
            | _ -> ())
        |> ignore
        connection.Connect()

    let activateCoffeePolice() =
        let favoriteCoffee (m : StatusMessage) statusID =
            if isInTime CoffeePolice.activeTime myTweetCoffeeTime m.Timestamp then
                async {
                    do! Twitter.favoriteTweet tokens statusID |> Async.Ignore
                    do Console.WriteLine("！ コーヒーふぁぼしました\n@{0} ({1})\n{2}\n", m.Status.User.ScreenName, toLocal m, m.Status.Text)
                } |> Async.Start
        Console.WriteLine("コーヒー警察起動")
        connection |> Observable.subscribe (function
            | Police.SuspendTweet(m) ->
                Console.WriteLine("コーヒー活動休止ツイート ({0}) : {1}\n", toLocal m, m.Status.Text)
                Volatile.Write(myTweetCoffeeTime, None)
            | CoffeePolice.StrictTweet(m, statusID) when Volatile.Read(myTweetCoffeeTime) |> Option.isNone ->
                Console.WriteLine("コーヒーは {0} まで活動\n", (m.Timestamp + CoffeePolice.activeTime).LocalDateTime)
                Volatile.Write(myTweetCoffeeTime , Some(m.Timestamp))
                favoriteCoffee m statusID
            | CoffeePolice.Tweet(m, statusID) ->
                favoriteCoffee m statusID
            | _ -> ())
        |> ignore
        connection.Connect()

        
    use shukaPolice = activateShukaPolice()
    use coffeePolice = activateCoffeePolice()

    System.Console.ReadLine() |> ignore
    0
