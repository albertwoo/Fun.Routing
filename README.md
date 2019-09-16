# Fun.Routing

This is a routing library for fable and inspired by Giraffe.

It supports both client side and server sider rendering.


# How to use
1. Add nuget package `Fun.Routing`
2. Define routes and use it with `update` function
```fsharp
let routes: Router<State, Cmd<Msg>> =
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
        match routes state url with
        | Some x -> x
        | None -> { state with CurrentPage = NotFound url }, Cmd.none
    ...
```
3. Use it in the ui. For client side there should be only one navigatorId and should be saved in the state. For the server sider there we should generate random id for concurrency safety.
```fsharp
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
```
4. For server side rendering please go to `demo/Server`


# How to run demo
1. Make sure you have `yarn`, `paket`, `fake-cli` installed globally
2. cd to `./demo` and run `fake build -t RunClient`