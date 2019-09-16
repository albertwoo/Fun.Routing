# Fun.Routing

This is a routing (navigation) library for fable and inspired by Giraffe.

It supports both client side and server sider rendering (SSR).


# How to use
1. Add nuget package `Fun.Routing`
2. Generate `RouterId` and save somewhere for example in app state. I use random id to avoid concurrency issue on server side rendering.
    ```fsharp
    let defaultState =
      { CurrentUrl = ""
        RouterId = Random().Next(0, 10000).ToString()
        ... }
    ```
3. Subscribe router change when app begin
    ```fsharp
    Cmd.batch [
      Cmd.ofSub (fun dispatch ->
        Navigator.subscribe navigatorId (fun url -> UrlChanged url |> dispatch))

      #if FABLE_COMPILER
      Cmd.ofMsg (UrlChanged "/")
      #endif
    ]
    ```
4. Define routes and use it with `update` function
    ```fsharp
    let routers: Router<State, Cmd<Msg>> =
        choose
          [
            routeCi ""          (fun state -> { state with CurrentPage = Home "Home" }, Cmd.none)
            routeCi "/"         (fun state -> { state with CurrentPage = Home "Home" }, Cmd.none)
            routeCi "/home"     (fun state -> { state with CurrentPage = Home "Home" }, Cmd.none)
            routeCi "/about"    (fun state -> { state with CurrentPage = About }, Cmd.none)
            routeCi "/notfound" (fun state -> { state with CurrentPage = NotFound "404" }, Cmd.none)
          ]

    let update msg (state: State) =
        match msg with
        | UrlChanged url ->
            let state = { state with CurrentUrl = url }
            match routers state url with
            | Some x -> x
            | None -> { state with CurrentPage = NotFound url }, Cmd.none
        ...
    ```
5. Use it in the ui.
    ```fsharp
    let ssrLink routerId link label =
      a [
        OnClick (fun e -> e.preventDefault(); Navigator.newUrl navigatorId link)
        Href link
        Classes [
          tailwind.``hover:text-green-700``
        ]
      ] [
        str label
      ] 
    ```
4. For server side rendering (SSR) implementation please go to `demo/Server`


# How to run demo
1. Make sure you have `dotnet sdk`, `yarn`, `paket`, `fake-cli` installed globally
2. cd to `./demo` and run `fake build -t RunClient`
3. cd to `./demo` and run `fake build -t RunServer` 