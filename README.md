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
3. For server side rendering please go to `demo/Server`