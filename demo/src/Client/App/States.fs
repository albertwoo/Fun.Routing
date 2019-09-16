module Client.App.States

open System
open Fable.Core.JsInterop
open Elmish
open Fun.Routing
open Fun.Routing.Router
open Client.Common


let defaultState =
  { RouterId = Random().Next(0, 10000).ToString()
    CurrentPage = Page.Loading
    CurrentDateTime = DateTime.Now }


let init() =
  let mutable routerId = ""
  (clientServerExec
    (fun s ->
      let initBaseStr = !!Browser.Dom.window?__INIT_STATE__: string
      let initState =
        if String.IsNullOrEmpty initBaseStr then s
        else
          match fromJson<State> (fromBase64 initBaseStr) with
          | Ok x -> x
          | Error e ->
              Browser.Dom.console.error e
              s
      routerId <- initState.RouterId
      initState)
    id
    defaultState)
  , Cmd.batch [
      Cmd.ofSub (fun dispatch -> Navigator.subscribe routerId (fun url -> UrlChanged url |> dispatch))

      #if FABLE_COMPILER
      Cmd.ofMsg (UrlChanged "/")
      #endif
    ]


let routes: Router<State, Cmd<Msg>> =
    choose
      [
        routeCi  ""          (fun state -> { state with CurrentPage = Home "Home" }, Cmd.none)
        routeCi  "/"         (fun state -> { state with CurrentPage = Home "Home" }, Cmd.none)
        routeCi  "/home"     (fun state -> { state with CurrentPage = Home "Home" }, Cmd.none)
        routeCi  "/about"    (fun state -> { state with CurrentPage = About }, Cmd.none)
        routeCi  "/blog"     (fun state -> { state with CurrentPage = Blog None }, Cmd.none)
        routeCif "/blog/%i"  (fun state id -> { state with CurrentPage = Blog (Some id) }, Cmd.none)
        routeCi  "/notfound" (fun state -> { state with CurrentPage = NotFound "404" }, Cmd.none)
      ]


let update msg (state: State) =
    match msg with
    | UrlChanged url ->
        match routes state url with
        | Some x -> x
        | None -> { state with CurrentPage = NotFound url }, Cmd.none

    | Tick ->
        { state with CurrentDateTime = DateTime.Now }, Cmd.none
