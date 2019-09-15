module Server.Handlers.SSRHandler


open System
open System.IO
open Giraffe
open Microsoft.AspNetCore.Hosting
open Microsoft.FSharpLu
open Elmish


let renderMainApp: HttpHandler =
  fun nxt ctx ->
    /// Init state
    /// Parse url
    /// Simulate program to finish model update
    /// Use model to create view element
    /// Render view element to html string
    let mutable stateResult: Client.App.Domain.State option = None
    let mutable viewResult: Fable.React.ReactElement option = None

    Program.mkProgram
      (fun a ->
        let state, cmd = Client.App.States.init a
        state
        , Cmd.batch [
            cmd
            Cmd.ofMsg (Client.App.Domain.UrlChanged ctx.Request.Path.Value)
          ])
      Client.App.States.update
      (fun model dispatch ->
        let v = Client.App.Views.app model dispatch
        stateResult <- Some model
        viewResult <- Some v
        v)
    |> Program.run

    let model = stateResult |> Option.defaultValue (Client.App.States.init() |> fst)
    let view = viewResult |> Option.defaultValue (Client.App.Views.app model ignore)

    let initState =
      sprintf """
              window.__INIT_STATE__ = "%s";
              """
              (Client.Common.Json.toJson model |> Text.encodeToBase64)

    let contentHtml = Fable.ReactServer.renderToString view

    let env = ctx.GetService<IHostingEnvironment>()

    let indexHtml =
      Path.Combine [| env.WebRootPath; "index.html" |]
      |> File.ReadAllText
      |> fun x ->
        x
         //.Replace("SERVER_SIDE_PLACEHOLDER_TITLE", model.PageTitle)
         .Replace("//SERVER_SIDE_PLACEHOLDER_STATE", initState)
         .Replace("<!--//SERVER_SIDE_PLACEHOLDER_HTML-->", contentHtml)

    htmlString indexHtml nxt ctx
