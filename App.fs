module App

open System
open Fable.Core
open Feliz
open Fable.React
open Elmish


JsInterop.import "" "@fontsource/roboto/300.css"
JsInterop.import "" "@fontsource/roboto/400.css"
JsInterop.import "" "@fontsource/roboto/500.css"
JsInterop.import "" "@fontsource/roboto/700.css"


[<Literal>]
let private themeDef =
    """createTheme({
  palette: {
    primary: {
        main: '#63C2C8',
    },
  },
})"""

[<Import("createTheme", from = "@mui/material/styles")>]
[<Emit(themeDef)>]
let private theme: obj = jsNative


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
        let state, dispatch = React.useElmish(TextField.init, TextField.update props.dispatch)

        let applySetText s =
            s |> TextField.Update |> dispatch
            s |> setSomeText

        JSX.jsx
            $"""
            import * from 'react';
            import Box from '@mui/material/Box';
            import Button from '@mui/material/Button';
            import Typography from '@mui/material/Typography';
            import TextField from '@mui/material/TextField';
            import Stack from '@mui/material/Stack';

            <React.Fragment>
                <TextField 
                    sx={ {| marginTop = theme.spacing 3 |} }
                    variant="standard" label={props.label} color="warning"
                    value={someText} onChange={OnChange.value >> applySetText}> </TextField>
            </React.Fragment>
        """


    [<JSX.Component>]
    let Counter (props: {| title: string; dispatch : int -> unit |}) =
        let (count, setCount) = React.useState 0

        JSX.jsx
            $"""
            import Box from '@mui/material/Box';
            import Button from '@mui/material/Button';
            import Typography from '@mui/material/Typography';
            import TextField from '@mui/material/TextField';

            <React.Fragment>
                <Typography variant="h1">{props.title}</Typography>
                <Typography>You clicked {count} times</Typography>
                <Button onClick={fun _ -> setCount (count + 1)}>Click me</Button>
            </React.Fragment>
        """


    [<JSX.Component>]
    let AppBar () =
        JSX.jsx
            $"""
        import AppBar from '@mui/material/AppBar';
        import Box from '@mui/material/Box';
        import Toolbar from '@mui/material/Toolbar';
        import Typography from '@mui/material/Typography';
        import Button from '@mui/material/Button';
        import IconButton from '@mui/material/IconButton';
        import MenuIcon from '@mui/icons-material/Menu';

            <Box sx={ {| flexGrow = 1 |} }>
                <AppBar position="static">
                <Toolbar>
                    <IconButton
                    size="large"
                    edge="start"
                    color="inherit"
                    aria-label="menu"
                    sx={ {| mr = 2 |} }
                    >
                    <MenuIcon />
                    </IconButton>
                    <Typography variant="h6" component="div" sx={ {| flexGrow = 1 |} }>
                    News
                    </Typography>
                    <Button color="inherit">Login</Button>
                </Toolbar>
                </AppBar>
            </Box>
        """


module private Root =
    
    type State = { Text : string; Count : int }

    type Msg = | TextUpdate of string | CountUpdate of int


    let init () = 
        { Text = ""; Count = 0 }, Cmd.none

    let update msg state =
        printfn "running Root update"
        match msg with
        | TextUpdate s -> { state with Text = s }, Cmd.none
        | CountUpdate c -> { state with Count = c }, Cmd. none



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
                            {| title = "Counter"; dispatch = ignore |}
                    }
                    </Box>
                    <Box>
                    {
                        Components.TextField
                            {| label = "Textfield"; text = "";  dispatch = ignore |}  
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
