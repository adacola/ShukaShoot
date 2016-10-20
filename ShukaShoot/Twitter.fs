module Adacola.ShukaShoot.Twitter

open System.Reactive.Linq
open CoreTweet
open CoreTweet.Streaming

let authenticate() =
    let auth = Model.config.Auth
    Tokens.Create(auth.ConsumerKey, auth.ConsumerSecret, auth.AccessToken, auth.AccessTokenSecret)

let getTimelineConnection (tokens : Tokens) =
    tokens.Streaming.UserAsObservable().Publish()

let favoriteTweet (tokens : Tokens) (statusID : int64) =
    tokens.Favorites.CreateAsync(statusID) |> Async.AwaitTask

let retweet (tokens : Tokens) (statusID : int64) =
    tokens.Statuses.RetweetAsync(statusID) |> Async.AwaitTask

let (|MyTweet|Tweet|OtherMessage|) (message : StreamingMessage) =
    match message with
    | :? StatusMessage as m ->
        if m.Status.User.Id |> Option.ofNullable |> Option.exists ((=) Model.config.Auth.UserId) then MyTweet(m)
        else Tweet(m)
    | _ -> OtherMessage(message)

let (|Retweet|_|) (message : StatusMessage) = message.Status.RetweetedStatus |> Option.ofObj

let toLocal (m : StatusMessage) = m.Timestamp.LocalDateTime

let isReplyTo userID (m : StatusMessage) = m.Status.InReplyToUserId |> Option.ofNullable |> Option.exists ((=) userID)
