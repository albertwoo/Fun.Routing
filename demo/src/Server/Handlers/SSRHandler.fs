module Server.Handlers.SSRHandler

open System
open System.IO
open Microsoft.AspNetCore.Hosting
open Microsoft.FSharpLu
open Giraffe
open Elmish


let private runApp<'State, 'Msg> (init: unit -> 'State * Cmd<'Msg>) update =
    let mutable currentState, cmd = init() 
    let rec run cmd =
        cmd 
        |> List.iter (fun sub ->
            sub (fun msg ->
                let newS, newC = update msg currentState
                currentState <- newS
                run newC
            )
        )
    run cmd
    currentState


let renderMainApp: HttpHandler =
    fun nxt ctx ->
        let model =
            try
                let url = ctx.Request.Path.Value + ctx.Request.QueryString.Value
                let initState, initCmd = Client.App.States.init()
                let state = runApp (fun _ -> initState, Cmd.batch [ initCmd; Cmd.ofMsg (Client.App.UrlChanged url) ]) Client.App.States.update
                Fun.Routing.Navigator.removeNavigators state.RouterId
                state
            with ex ->
                { Client.App.States.defaultState with Error = Some (string ex) }

        let view = Client.App.Views.app model ignore

        let finalState =
            sprintf """
                    window.__INIT_STATE__ = "%s";
                    """
                    (Client.Common.Json.toJson model |> Text.encodeToBase64)

        let contentHtml = Fable.ReactServer.renderToString view

        let env = ctx.GetService<IWebHostEnvironment>()

        let indexHtml =
            Path.Combine [| env.WebRootPath; "index.html" |]
            |> File.ReadAllText
            |> fun str ->
                str.Replace("//SERVER_SIDE_PLACEHOLDER_STATE", finalState)
                   .Replace("<!--//SERVER_SIDE_PLACEHOLDER_HTML-->", contentHtml)

        htmlString indexHtml nxt ctx
