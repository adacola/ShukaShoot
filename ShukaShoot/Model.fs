module Adacola.ShukaShoot.Model

open FSharp.Data

type Config = JsonProvider<"configSample.json">
let private configFileName = "config.json"
let config = Config.Load(configFileName)
