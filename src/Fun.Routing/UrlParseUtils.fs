module Fun.Routing.UrlParseUtils

open System
open System.Text.RegularExpressions
open Microsoft.FSharp.Reflection


let private formatStringMap =
    let decodeSlashes (str : string) =
        // Kestrel has made the weird decision to
        // partially decode a route argument, which
        // means that a given route argument would get
        // entirely URL decoded except for '%2F' (/).
        // Hence decoding %2F must happen separately as
        // part of the string parsing function.
        //
        // For more information please check:
        // https://github.com/aspnet/Mvc/issues/4599
        str.Replace("%2F", "/").Replace("%2f", "/")
    
    let guidPattern =
        "([0-9A-Fa-f]{8}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{4}\-[0-9A-Fa-f]{12}|[0-9A-Fa-f]{32}|[-_0-9A-Za-z]{22})"

    let boolParser = function
        | "true"
        | "1" -> true
        | "false"
        | "0" -> false
        | x -> failwithf "Not supported bool value %s" x

    dict [
    // Char    Regex                    Parser
    // -------------------------------------------------------------
        'b', ("(true|false){1}",        boolParser           >> box)  // bool
        'c', ("(.{1})",                 char                 >> box)  // char
        's', ("(.+)",                   decodeSlashes        >> box)  // string
        'i', ("(-?\d+)",                int32                >> box)  // int
        'd', ("(-?\d+)",                int64                >> box)  // int64
        'f', ("(-?\d+\.{0,1}\d+)",      float                >> box)  // float
        'O', (guidPattern,              Guid.Parse           >> box)  // Guid
    ]


let private convertToRegexPatternAndFormatChars (formatString : string) =
    let rec convert (chars : char list) =
        match chars with
        | '%' :: '%' :: tail ->
            let pattern, formatChars = convert tail
            "%" + pattern, formatChars
        | '%' :: c :: tail ->
            let pattern, formatChars = convert tail
            let regex, _ = formatStringMap.[c]
            regex + pattern, c :: formatChars
        | c :: tail ->
            let pattern, formatChars = convert tail
            c.ToString() + pattern, formatChars
        | [] -> "", []

    formatString.ToCharArray()
    |> Array.toList
    |> convert
    |> (fun (pattern, formatChars) -> sprintf "^%s$" pattern, formatChars)


let tryMatchInput (format : PrintfFormat<_,_,_,_, 'T>) (input : string) (ignoreCase : bool) =
    let pattern, formatChars =
        format.Value
        |> Regex.Escape
        |> convertToRegexPatternAndFormatChars

    let options =
        match ignoreCase with
        | true  -> RegexOptions.IgnoreCase
        | false -> RegexOptions.None

    let result = Regex.Match(input, pattern, options)

    if isNull result || result.Groups.Count <= 1
    then None
    else
        let groups = result.Groups |> Seq.cast<Group> |> Seq.skip 1

        let values =
            (groups, formatChars)
            ||> Seq.map2 (fun g c ->
                let _, parser = formatStringMap.[c]
                parser g.Value)
            |> Seq.toArray

        match values.Length with
            | 1 -> values.[0]
            | _ ->
                let tupleType =
                    values
                    |> Array.map (fun v ->
                        #if FABLE_COMPILER
                        typeof<obj>
                        #else
                        v.GetType()
                        #endif
                        )
                    |> FSharpType.MakeTupleType
                FSharpValue.MakeTuple(values, tupleType)
        |> unbox<'T> |> Some
