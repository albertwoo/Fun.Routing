namespace Fun.Routing

open Router


type NavigationState =
  { Event: Event<RouteUrl>
    mutable UrlStack: string list
    mutable CurrentUrl: string option
    mutable OnChangeRef: Handler<string> }
  static member defaultValue =
    {
      Event = new Event<string>()
      UrlStack = []
      CurrentUrl = None
      OnChangeRef =
        Handler(fun _ ->
          failwith "`OnChangeRef` has not been initialized.")
    }


module Navigator =
    let mutable navigators: Map<string, NavigationState> = Map.empty

    let getNavigator id =
      navigators
      |> Map.tryFind id
      |> function
        | Some x -> x
        | None ->
            let navigator = NavigationState.defaultValue
            navigators <- navigators |> Map.add id navigator
            navigator          

    let modifyUrl id (newUrl:string) =
        let navigator = getNavigator id
        navigator.UrlStack <-
            match navigator.UrlStack with
            | [] -> [ newUrl ]
            | _  -> navigator.UrlStack.GetSlice(Some 0, Some (navigator.UrlStack.Length - 1))@[newUrl]
        navigator.CurrentUrl <- Some newUrl
        #if FABLE_COMPILER
        Browser.Dom.window.history.replaceState((), "", newUrl)
        #else
        ()
        #endif
        navigator.Event.Trigger newUrl

    let newUrl id (newUrl:string): _ =
        let navigator = getNavigator id
        navigator.UrlStack <- navigator.UrlStack@[newUrl]
        navigator.CurrentUrl <- Some newUrl
        #if FABLE_COMPILER
        Browser.Dom.window.history.pushState((), "", newUrl)
        #else
        ()
        #endif
        navigator.Event.Trigger newUrl


    let subscribe id dispatch =
        let navigator = getNavigator id
        let handleUrlChange _ url = dispatch url
        navigator.OnChangeRef <- Handler handleUrlChange
        navigator.Event.Publish.AddHandler navigator.OnChangeRef

        #if FABLE_COMPILER
        Browser.Dom.window.onpopstate <- fun _ ->
          navigator.UrlStack <-
            match navigator.UrlStack with
            | [] -> []
            | _  -> navigator.UrlStack |> List.take (navigator.UrlStack.Length - 1)
          navigator.CurrentUrl <- List.tryLast navigator.UrlStack
          Browser.Dom.window.location.href.Substring(Browser.Dom.window.location.origin.Length) |> dispatch
        #endif


    let unsubscribe id =
        let navigator = getNavigator id
        navigator.Event.Publish.RemoveHandler navigator.OnChangeRef


    let removeNavigators id = navigators <- navigators |> Map.filter (fun k _ -> k = id)
