module Client.Startup

open Elmish
open Elmish.React


Program.mkProgram App.States.init App.States.update App.Views.app
|> Program.withReactSynchronous "root"
|> Program.withConsoleTrace
|> Program.run