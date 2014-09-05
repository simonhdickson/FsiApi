namespace FsiApi
open System.IO
open System.Net.Http.Formatting
open System.Net.Http.Headers
open System.Threading.Tasks

type TextMediaTypeFormatter() as this =
    inherit MediaTypeFormatter()
    do this.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"))

    override  __.ReadFromStreamAsync(_, readStream, _, _) =
        let taskCompletionSource = new TaskCompletionSource<obj>();
        try
            let memoryStream = new MemoryStream()
            readStream.CopyTo(memoryStream)
            let s = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray())
            taskCompletionSource.SetResult(s)
        with
            | e -> taskCompletionSource.SetException(e)
        taskCompletionSource.Task

    override __.WriteToStreamAsync(_, value, writeStream, _, _) =
        let taskCompletionSource = new TaskCompletionSource<obj>();
        try
            let serialized = System.Text.Encoding.Default.GetBytes(value :?> string)
            writeStream.Write(serialized, 0, serialized.Length)
            taskCompletionSource.SetResult()
        with
            | e -> taskCompletionSource.SetException(e)
        taskCompletionSource.Task :> Task
 
    override __.CanReadType ``type`` = ``type`` = typeof<string>
 
    override __.CanWriteType ``type`` = ``type`` = typeof<string>
