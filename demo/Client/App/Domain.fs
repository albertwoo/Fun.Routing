[<AutoOpen>]
module rec Client.App.Domain

open System


type State =
  { CurrentUrl: string
    NavigatorId: string
    CurrentPage: Page
    CurrentDateTime: DateTime }

type Msg =
  | UrlChanged of string
  | Tick


type Page =
  | Home of string
  | About
  | Loading
  | NotFound of string
