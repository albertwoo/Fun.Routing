module Client.App.Views

open System
open Fable.React
open Fable.React.Props
open Zanaptak.TypedCssClasses
open Fun.Routing


type tailwind = CssClasses<"./public/css/tailwind-generated.css", Naming.Verbatim>
type fontAwsome = CssClasses<"./public/css/font-awesome-v5-10-2.min.css", Naming.Verbatim>


let ssrLink navigatorId link label =
    a </> [
        OnClick (fun e -> e.preventDefault(); Navigator.newUrl navigatorId link)
        Href link
        Classes [ tailwind.``hover:text-green-700`` ]
        Text label
    ]
    
let solidIcon classes =
    span </> [
        Classes [
            fontAwsome.fas; tailwind.``text-gray-800``
            yield! classes
        ]
    ] 


let app state dispatch =
    let currentPage =
        match state.CurrentPage with
        | Page.Home x       -> h1 </> [ Text x ]
        | Page.About        -> h1 </> [ Text "About" ]
        | Page.Blog id      -> h1 </> [ Text (match id with Some id -> sprintf "Selected blog: %d" id | None -> "Blogs")  ]
        | Page.Doc q        -> h1 </> [ Text (sprintf "Doc %s " q) ]
        | Page.FormatTest q -> h1 </> [ Text (sprintf "Format %s " q) ]
        | Page.Loading      -> solidIcon [ fontAwsome.``fa-truck-loading`` ]
        | Page.NotFound x   -> h1 </> [ Text (sprintf "404 NOT FOUND: %s" x) ]

    div </> [
        Classes [ tailwind.``font-sans`` ]
        Children [
            div </> [
                Classes [ tailwind.``text-center`` ]
                Children [
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
            ]

            div </> [
                Classes [ tailwind.``text-center``; tailwind.``text-4xl`` ]
                Children [ currentPage ]
            ]
        ]
    ]
