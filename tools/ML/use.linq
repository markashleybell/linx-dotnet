<Query Kind="FSharpProgram">
  <Reference Relative="..\..\src\ml\bin\Debug\net5.0\ml.dll">C:\Src\linx-dotnet\src\ml\bin\Debug\net5.0\ml.dll</Reference>
  <NuGetReference>Microsoft.ML</NuGetReference>
  <Namespace>LinxML.Common</Namespace>
  <Namespace>Microsoft.ML</Namespace>
  <Namespace>Microsoft.ML.Calibrators</Namespace>
  <Namespace>Microsoft.ML.Data</Namespace>
  <Namespace>Microsoft.ML.Runtime</Namespace>
  <Namespace>Microsoft.ML.Trainers</Namespace>
  <Namespace>Microsoft.ML.Transforms</Namespace>
  <Namespace>Microsoft.ML.Transforms.Text</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

let (dataPath, modelPath) = Util.CurrentQueryPath |> getDataAndModelPaths

let (model, schema) = ctx.Model.Load(modelPath)

let engine = ctx.Model.CreatePredictionEngine<Link, Link>(model)

let test1 = {
    Title = "deploy app with docker"
    Abstract = "containerising csharp asp.net applications"
    Tags = ""
}

let test2 = {
    Title = "bbc micro"
    Abstract = "retro computing hardware with bbc micro"
    Tags = ""
}

let test3 = {
    Title = "azure blob csharp"
    Abstract = "storage accounts blob cloud azure .net"
    Tags = ""
}

[|test1;test2;test3|] 
|> Array.map (fun t -> (t.Title, engine.Predict(t).Tags))
|> Dump
|> ignore
