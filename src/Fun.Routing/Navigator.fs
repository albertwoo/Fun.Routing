namespace Fun.Routing

open Router


type NavigationState =
  { Event: Event<RouteUrl>
    mutable UrlStack: string list
    mutable LastUrl: string option
    mutable CurrentUrl: string option
    mutable OnChangeRef: Handler<string> }
  static member defaultValue =
    {
      Event = new Event<string>()
      UrlStack = []
      LastUrl = None
      CurrentUrl = None
      OnChangeRef =
        Handler(fun _ ->
          failwith "`onChangeRef` has not been initialized.\nPlease make sure you used Elmish.Navigation.Program.Internal.subscribe")
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
        let handleUrlChange _ url =
            match navigator.LastUrl with
            | Some href when href = url -> ()
            | _ ->
                navigator.LastUrl <- Some url
                dispatch url
        navigator.OnChangeRef <- Handler handleUrlChange
        navigator.Event.Publish.AddHandler navigator.OnChangeRef

    let unsubscribe id =
        let navigator = getNavigator id
        navigator.Event.Publish.RemoveHandler navigator.OnChangeRef
