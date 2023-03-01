module App

open System
open Fable.Core
open Feliz
open Fable.React
open Elmish

JsInterop.importSideEffects "@fontsource/roboto/300.css"
JsInterop.importSideEffects "@fontsource/roboto/400.css"
JsInterop.importSideEffects "@fontsource/roboto/500.css"
JsInterop.importSideEffects "@fontsource/roboto/700.css"

[<Import("createTheme", from = "@mui/material/styles")>]
let createTheme (theme: obj) : obj = jsNative

let theme = createTheme {|
    palette = {| 
        primary = {| main = "#63C2C8" |} 
    |}
|}


[<Erase>]
type Mui = 
    [<ReactComponent(import="default", from="@mui/material/Button")>]
    static member Button(?children: string, ?color: string, ?onClick: unit -> unit) : ReactElement = jsNative
    
    [<ReactComponent(import="default", from="@mui/material/Typography")>]
    static member Typography(?children: string, ?variant: string): ReactElement = jsNative


module private Components =

    open Elmish
    open Fable.React
    open Utils


    module TextField =

        type State = string

        type Msg = | Update of string | Clear

        let init () = "", Cmd.none

        let update dispatch msg state =
            match msg with
            | Clear    -> "", Cmd.none
            | Update s -> 
                printfn $"received an update: {s}"
                // calling parent dispatch
                s |> dispatch
                s, Cmd.none


    [<JSX.Component>]
    let TextField (props: {| label : string; text : string; dispatch : string -> unit |}) =
        let theme = Mui.useTheme ()

        let (someText, setSomeText) = React.useState props.text
        let state, dispatch = React.useElmish(TextField.init, TextField.update props.dispatch, [| box props.dispatch |])

        let applySetText s =
            s |> TextField.Update |> dispatch
            s |> setSomeText

        JSX.jsx
            $"""
            import * from 'react';
            import Box from '@mui/material/Box';
            import TextField from '@mui/material/TextField';
            import Stack from '@mui/material/Stack';

            <React.Fragment>
                <TextField 
                    sx={ {| marginTop = theme.spacing 3 |} }
                    variant="standard" label={props.label} color="warning"
                    value={someText} onChange={OnChange.value >> applySetText}> </TextField>
            </React.Fragment>
        """


    [<ReactComponent>]
    let Counter (props: {| title: string; dispatch : int -> unit |}) =
        let (count, setCount) = React.useState 0

        let applyCount () =  
            count + 1 |> setCount
            count + 1 |> props.dispatch

        React.fragment [
            Mui.Typography(props.title, variant="h1")
            Mui.Typography($"You clicked {count} times")
            Mui.Button("Click me", onClick = fun _ -> applyCount ())
        ]

    [<ReactComponent(import="ApplicationBar", from="./ApplicationBar.jsx")>]
    let AppBar(): ReactElement = jsNative 

module private Root =
    
    type State = { Text : string; Count : int }

    type Msg = | TextUpdate of string | CountUpdate of int


    let init () = 
        { Text = ""; Count = 0 }, Cmd.none

    let update msg state =
        match msg with
        | TextUpdate s -> 
            printfn $"text update: {s}"
            { state with Text = s }, Cmd.none
        | CountUpdate c -> 
            printfn $"count update: {c}"
            { state with Count = c }, Cmd. none



[<JSX.Component>]
let Root () =
    let state, dispatch = React.useElmish(Root.init, Root.update)

    printfn $"root state is now: {state}"

    JSX.jsx
        $"""
    import {{ ThemeProvider}} from '@mui/material/styles';
    import CssBaseline from '@mui/material/CssBaseline';
    import React from "react";
    import Stack from '@mui/material/Stack';
    import Container from '@mui/material/Container';

    <React.StrictMode>  
        <ThemeProvider theme={theme}>
            <CssBaseline/>
                <Container>
                {Components.AppBar()}
                <Stack>
                    <Box>
                    {
                        Components.Counter 
                            {| title = "Counter"; dispatch = Root.CountUpdate >> dispatch |}
                    }
                    </Box>
                    <Box>
                    {
                        Components.TextField
                            {| label = "Textfield"; text = "";  dispatch = Root.TextUpdate >> dispatch |}  
                    }
                    </Box>
                </Stack>
                </Container>
        </ThemeProvider>
    </React.StrictMode>
    """

open Browser.Dom

document.getElementById "root"
|> ReactDOM.createRoot
|> fun r -> r.render (Root() |> unbox)
