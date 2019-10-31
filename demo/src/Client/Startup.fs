module Client.Startup

open Elmish
open Elmish.React

#if DEBUG
open Elmish.HMR
#endif


Program.mkProgram App.States.init App.States.update App.Views.app
|> Program.withSubscription App.States.routerSub
#if DEBUG
|> Program.withConsoleTrace
#endif
|> Program.withReactBatched "root"
#if DEBUG
#endif
|> Program.run