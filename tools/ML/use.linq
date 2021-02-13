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
let defaultLabelColumn = "Label"
[<Literal>]
let defaultFeaturesColumn = "Features"
[<Literal>]
let defaultPredictedLabelColumn = "PredictedLabel"

let dataPath = @"C:\Src\LinkCategoriser\model.zip"

[<CLIMutable>]
type Link = {
    [<LoadColumn(0)>] ID : string
    [<LoadColumn(1)>] Title : string
    [<LoadColumn(2)>] Body : string
    [<LoadColumn(3)>] Category : string
}

[<CLIMutable>]
type CategoryPrediction = {
    [<ColumnName(defaultPredictedLabelColumn)>] Category : string
}

let ctx = MLContext(seed = Nullable 0)

let (model, schema) = ctx.Model.Load(dataPath)

let engine = ctx.Model.CreatePredictionEngine<Link, CategoryPrediction>(model)

let test = {
    ID = ""
    Title = "stories"
    Body = "https://amazon.co.uk"
    Category = ""
}

let prediction = engine.Predict(test)

prediction.Dump()
