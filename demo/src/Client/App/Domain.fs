namespace rec Client.App

open System


type State =
  { RouterId: string
    CurrentPage: Page
    CurrentDateTime: DateTime
    Error: string option }

type Msg =
  | UrlChanged of string
  | Tick


type Page =
  | Home of string
  | About
  | Blog of int option
  | Doc of query: string
  | FormatTest of string
  | Loading
  | NotFound of string
