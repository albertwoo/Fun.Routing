module Fun.Routing.Router


type RouteUrl = string

type Router<'State, 'Cmd> = 'State -> RouteUrl -> ('State * 'Cmd) Option


let choose<'State, 'Cmd> (routes: Router<'State, 'Cmd> list): Router<'State, 'Cmd> =
    fun state url ->
        routes
        |> List.tryPick (fun router -> router state url)


let routeCi (pattern: string) update: Router<_, _> =
    fun state url ->
        if url.Length = pattern.Length && url.ToLower().StartsWith (pattern.ToLower()) then
            Some (update state)
        else
            None


let routeCif (path: PrintfFormat<_,_,_,_, 'T>) update: Router<_, _> =
    fun state url ->
        UrlParseUtils.tryMatchInput path url true
        |> Option.map (update state)


let routeCifWithQuery (path: PrintfFormat<_,_,_,_, 'T>) update: Router<_, _> =
    fun state url ->
        let spliterIndex = url.IndexOf '?'
        let newUrl =
            if spliterIndex > -1 then url.Substring(0, spliterIndex)
            else url
        let query =
            if spliterIndex > -1 then Some (url.Substring(spliterIndex + 1))
            else None
        UrlParseUtils.tryMatchInput path newUrl true
        |> Option.map (fun v -> update state v query)
