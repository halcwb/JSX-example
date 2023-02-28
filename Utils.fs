module Utils

[<RequireQualifiedAccess>]
module OnChange =
    open Fable.Core.JsInterop

    let inline value<'t>  (e: Browser.Types.Event) = e.target?value |> unbox<'t>
