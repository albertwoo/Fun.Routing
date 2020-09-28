[<AutoOpen>]
module Client.Controls.Common

open Fable.React
open Fable.React.Props
open Zanaptak.TypedCssClasses


type tailwind = CssClasses<"./public/css/tailwind-generated.css", Naming.Verbatim>
type fontAwsome = CssClasses<"./public/css/font-awesome-v5-10-2.min.css", Naming.Verbatim>



[<RequireQualifiedAccess>]
module Heading =
    let h1 classes (txt: string) =
        div </> [
            Classes [
                tailwind.``text-3xl``; tailwind.``font-bold``
                yield! classes
            ]
            Text txt 
        ]

    let h2 classes (txt: string) =
        div [
            Classes [
                tailwind.``text-2xl``; tailwind.``font-semibold``
                yield! classes
            ]
            Text txt 
        ]


[<RequireQualifiedAccess>]
module Layout =
    let level classes childs =
        div </> [
            Classes [
                tailwind.``mx-auto``; tailwind.flex; tailwind.``flex-no-wrap``; tailwind.``items-center``; tailwind.``justify-center``
                yield! classes
            ]
            Children childs
        ]

    let spacer classes =
        span </> [
            Classes [
                tailwind.``px-02``
                yield! classes
            ]
        ]


[<RequireQualifiedAccess>]
module Icons =
    let brand classes =
        span </> [
            Classes [
            fontAwsome.fab; tailwind.``text-gray-800``
            yield! classes
            ]
        ]

    let solid classes =
        span [
            Classes [
                fontAwsome.fas; tailwind.``text-gray-800``
                yield! classes
            ]
        ]

    let urlIcon url icon =
        match url with
        | Some url ->
            a </> [
                Href url
                Children [ icon ]
            ]
        | None ->
            emptyView
