﻿module Adacola.ShukaShoot.Twitter

open System.Reactive.Linq
open CoreTweet
open CoreTweet.Streaming
open FSharp.Data

type Auth = JsonProvider<"authSample.json">
let private authFileName = "auth.json"
let private auth = Auth.Load(authFileName)

let authenticate() =
    Tokens.Create(auth.ConsumerKey, auth.ConsumerSecret, auth.AccessToken, auth.AccessTokenSecret)

let getTimelineConnection (tokens : Tokens) =
    tokens.Streaming.UserAsObservable().Publish()

let favoriteTweet (tokens : Tokens) (statusID : int64) =
    tokens.Favorites.CreateAsync(statusID) |> Async.AwaitTask

let (|MyTweet|Tweet|OtherMessage|) (message : StreamingMessage) =
    match message with
    | :? StatusMessage as m ->
        if m.Status.User.Id |> Option.ofNullable |> Option.exists ((=) auth.UserId) then MyTweet(m)
        else Tweet(m)
    | _ -> OtherMessage(message)

let (|Retweet|_|) (message : StatusMessage) = message.Status.RetweetedStatus |> Option.ofObj

let toLocal (m : StatusMessage) = m.Timestamp.LocalDateTime
