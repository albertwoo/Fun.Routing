module Server.Routes

open Giraffe
open Server.Handlers


let mainRoutes: HttpHandler =
    choose
        [
            GET >=> SSRHandler.renderMainApp
        ]

