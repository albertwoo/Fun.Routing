module Fun.Routing.Router


type RouteUrl = string

type Router<'State, 'Cmd> = 'State -> RouteUrl -> ('State * 'Cmd) Option

/// Find the first matched router in the list
let choose<'State, 'Cmd> (routes: Router<'State, 'Cmd> list): Router<'State, 'Cmd> =
    fun state url ->
        routes
        |> List.tryPick (fun router -> router state url)


/// Check the start of the url for the parttern and ignore case sensitive
let subRouteCi (pattern: string) routes: Router<_, _> =
    fun state url ->
        if url.ToLower().StartsWith (pattern.ToLower()) then
            choose routes state (url.Substring(pattern.Length))
        else
            None


/// Exact match the url and ignore case sensitive
let routeCi (pattern: string) update: Router<_, _> =
    fun state url ->
        if url.ToLower() = pattern.ToLower() then
            Some (update state)
        elif (url = "" || url = "/") && pattern = "" then
            Some (update state)
        else
            None

/// Match the url, extract parameters and ignore case sensitive
let routeCif (path: PrintfFormat<_,_,_,_, 'T>) update: Router<_, _> =
    fun state url ->
        UrlParseUtils.tryMatchInput path url true
        |> Option.map (update state)

        
/// Match the url, extract parameters and query strings and ignore case sensitive
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


/// Match any url
let routeAny update: Router<_, _> =
    fun state url ->
        Some (update state url)
