<Query Kind="FSharpProgram">
  <NuGetReference>Microsoft.ML</NuGetReference>
  <Namespace>Microsoft.ML</Namespace>
  <Namespace>Microsoft.ML.Calibrators</Namespace>
  <Namespace>Microsoft.ML.Data</Namespace>
  <Namespace>Microsoft.ML.Runtime</Namespace>
  <Namespace>Microsoft.ML.Trainers</Namespace>
  <Namespace>Microsoft.ML.Transforms</Namespace>
  <Namespace>Microsoft.ML.Transforms.Text</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

[<Literal>]
let defaultPredictedLabelColumn = "PredictedLabel"

let dataPath = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "testdata")

let modelPath = Path.Combine(dataPath, "model.zip")

[<CLIMutable>]
type Link = {
    [<LoadColumn(0)>] Title : string
    [<LoadColumn(1)>] Abstract : string
    [<LoadColumn(2)>] Tags : string
}

[<CLIMutable>]
type TagsPrediction = {
    [<ColumnName(defaultPredictedLabelColumn)>] Tags : string
}

let ctx = MLContext(seed = Nullable 0)

let (model, schema) = ctx.Model.Load(modelPath)

let engine = ctx.Model.CreatePredictionEngine<Link, TagsPrediction>(model)

let test = {
    Title = "bbc micro"
    Abstract = "retro computing hardware with bbc micro"
    Tags = ""
}

let prediction = engine.Predict(test)

prediction.Dump()
