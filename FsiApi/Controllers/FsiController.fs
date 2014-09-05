namespace FsiApi.Controllers
open System.Web.Http
open Microsoft.FSharp.Compiler.Interactive.Shell

module Fsi =
    open System.IO
    open System.Text

    let sbOut = new StringBuilder()
    let sbErr = new StringBuilder()
    let inStream = new StringReader("")
    let outStream = new StringWriter(sbOut)
    let errStream = new StringWriter(sbErr)

    let argv = [| "C:\\fsi.exe" |]
    let allArgs = Array.append argv [|"--noninteractive"|]

    let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
    let session = FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, outStream, errStream)

    let eval expression = 
        try 
            match session.EvalExpression(expression) with
            | Some value -> sprintf "%A" value.ReflectionValue 
            | None -> "evaluation produced nothing."
        with e -> e.ToString()

/// Retrieves values.
type FsiController() =
    inherit ApiController()

    [<Route("fsi/eval")>]
    [<HttpPost>]
    member __.Eval([<FromBody>] text:string) =
        let request =
            text.Split([|'\n'|])
            |> Seq.map (fun i -> let x = i.Split('=')
                                 x.[0], x.[1])
            |> Map.ofSeq
        Fsi.eval request.["text"]