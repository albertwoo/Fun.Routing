module Client.App.Views

open System
open Fable.React
open Fable.React.Props
open Fun.Routing
open Zanaptak.TypedCssClasses


type tailwind = CssClasses<"./public/css/tailwind-generated.css", Naming.Verbatim>
type fontAwsome = CssClasses<"./public/css/font-awesome-v5-10-2.min.css", Naming.Verbatim>


let Classes str = str |> List.filter (String.IsNullOrEmpty >> not) |> String.concat " " |> Class

let emptyView = div [ Style [ Display DisplayOptions.None ] ] []

let ssrLink navigatorId link label =
  a [
    OnClick (fun e -> e.preventDefault(); Navigator.newUrl navigatorId link)
    Href link
    Classes [
      tailwind.``hover:text-green-700``
    ]
  ] [
    str label
  ] 
    
let solidIcon classes =
  span [
    Classes [
      yield! classes
      yield fontAwsome.fas
      yield tailwind.``text-gray-800``
    ]
  ] []


let app state dispatch =
    let currentPage =
        match state.CurrentPage with
        | Page.Home x     -> h1 [] [ str x ]
        | Page.About      -> h1 [] [ str "About" ]
        | Page.Loading    -> solidIcon [ fontAwsome.``fa-truck-loading`` ]
        | Page.NotFound x -> h1 [] [ str (sprintf "%s %s" x state.CurrentUrl) ]

    div [
      Classes [
        tailwind.``font-sans``
      ]
    ] [
      div [
        Classes [
          tailwind.``text-center``
        ]
      ] [
        ssrLink state.NavigatorId "/" "Home"
        str " / "
        ssrLink state.NavigatorId "/about" "About"
      ]

      div [
        Classes [
          tailwind.``text-center``
          tailwind.``text-4xl``
        ]
      ] [
        currentPage
      ]
    ]
