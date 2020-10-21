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
      CurrentDateTime = DateTime.Now
      Error = None }


let init() =
    let state =
        clientServerExec
            (fun s ->
                let initBaseStr = !!Browser.Dom.window?__INIT_STATE__: string
                if String.IsNullOrEmpty initBaseStr then s
                else
                    match fromJson<State> (fromBase64 initBaseStr) with
                    | Ok x -> x
                    | Error e ->
                        Browser.Dom.console.error e
                        s)
                id
            defaultState
    state
  , Cmd.batch [
        #if FABLE_COMPILER
        Cmd.ofMsg (UrlChanged "/")
        #endif
    ]


let routerSub state =
    Cmd.ofSub (fun dispatch ->
        Fun.Routing.Navigator.subscribe state.RouterId (UrlChanged >> dispatch)
    )


type MyRouter = Router<State * Cmd<Msg>, State * Cmd<Msg>>


let logUrl: MyRouter =
    fun s url ->
        #if FABLE_COMPILER
        Browser.Dom.console.error url
        #endif
        Some s


let routeUpdate: MyRouter =
    choose [
        routeCi  ""                   (fun (state, _) -> { state with CurrentPage = Home "Home" }, Cmd.none) >=> logUrl
        routeCi  "/home"              (fun (state, _) -> { state with CurrentPage = Home "Home" }, Cmd.none)
        routeCi  "/about"             (fun (state, _) -> { state with CurrentPage = About }, Cmd.none)
        routeCi  "/#/about"           (fun (state, _) -> { state with CurrentPage = About }, Cmd.none)

        subRouteCi "/blog" [
            routeCi  ""               (fun (state, _) -> { state with CurrentPage = Blog None }, Cmd.none)
            routeCif "/%i"            (fun (state, _) id -> { state with CurrentPage = Blog (Some id) }, Cmd.none)
        ]

        routeCifWithQuery "/doc/%d"   (fun (state, _) (id: int64) q -> { state with CurrentPage = Doc (q |> Option.defaultValue "defa") }, Cmd.none)
        
        routeCif "/exception/%i"   (fun (state, _) (_: int64) -> state, Cmd.none)

        routeCif "/tuple/t1/%i/t2/%f" (fun (state, _) (t1, t2) -> { state with CurrentPage = FormatTest (sprintf "%i - %f" t1 t2) }, Cmd.none)
        routeCif "/tuple/t1/%i/t2/%s" (fun (state, _) (t1, t2) -> { state with CurrentPage = FormatTest (sprintf "%i - %s" t1 t2) }, Cmd.none)
        routeCif "/format/%b/%c/%s/%i/%d/%f/%O" (fun (state, _) (b, c, s, i, (d: int64), f, (o: Guid)) -> { state with CurrentPage = FormatTest (sprintf "%b/%c/%s/%i/%d/%f/%O" b c s i d f o) }, Cmd.none)

        routeAny                      (fun (state, _) url -> { state with CurrentPage = NotFound url }, Cmd.none)
    ]


let update msg (state: State) =
    match msg with
    | UrlChanged url ->
        match routeUpdate (state, Cmd.none) url with
        | Some x -> x
        | None -> { state with CurrentPage = NotFound url }, Cmd.none

    | Tick ->
        { state with CurrentDateTime = DateTime.Now }, Cmd.none
