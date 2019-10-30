module Client.App.Views

open System
open Fable.React
open Fable.React.Props
open Zanaptak.TypedCssClasses
open Fun.Routing


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
        | Page.Home x       -> h1 [] [ str x ]
        | Page.About        -> h1 [] [ str "About" ]
        | Page.Blog id      -> h1 [] [ str (match id with Some id -> sprintf "Selected blog: %d" id | None -> "Blogs")  ]
        | Page.Doc q        -> h1 [] [ str (sprintf "Doc %s " q) ]
        | Page.FormatTest q -> h1 [] [ str (sprintf "Format %s " q) ]
        | Page.Loading      -> solidIcon [ fontAwsome.``fa-truck-loading`` ]
        | Page.NotFound x   -> h1 [] [ str (sprintf "404 NOT FOUND: %s" x) ]

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
        ssrLink state.RouterId "/" "Home"
        hr[]
        ssrLink state.RouterId "/about" "About"
        hr[]
        ssrLink state.RouterId "/#/about" "# About"
        hr[]
        ssrLink state.RouterId "/blog" "Blog"
        hr[]
        ssrLink state.RouterId "/blog/12" "Blog 12"
        hr[]
        ssrLink state.RouterId "/blog/13?title=中文？" "Blog with query"
        hr[]
        ssrLink state.RouterId "/doc/4" "Doc with query"
        hr[]
        ssrLink state.RouterId "/doc/4?title=Test title" "Doc with query2"
        hr[]
        ssrLink state.RouterId "/tuple/t1/3/t2/4" "Should be: 3 4"
        hr[]
        ssrLink state.RouterId "/tuple/t1/5/t2/6.4" "Should be: 5 6.4"
        hr[]
        ssrLink state.RouterId "/tuple/t1/7/t2/test" "Should be: 5 test"
        hr[]
        ssrLink state.RouterId "/format/true/w/test/1/2/3.4/e8ff4edc-31e3-4683-b801-ad36eeee404e" "Format test. Should be: true/w/test/1/2/3.4/e8ff4edc-31e3-4683-b801-ad36eeee404e"
        hr[]
        ssrLink state.RouterId "/test404" "Test 404"
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
