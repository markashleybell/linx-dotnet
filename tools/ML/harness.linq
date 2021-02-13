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

let dataPath = Path.GetDirectoryName(Util.CurrentQueryPath) + @"\testdata"

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

let data = ctx.Data.LoadFromTextFile<Link>(dataPath + @"\data.csv", hasHeader = true, allowQuoting = true, separatorChar = ',')

// data.Preview(10).Dump()

let dataPartitions = ctx.Data.TrainTestSplit(data, testFraction = 0.2)

let trainingData = dataPartitions.TrainSet
let testData = dataPartitions.TestSet

let pipeline = EstimatorChain()
                .Append(ctx.Transforms.Conversion.MapValueToKey(inputColumnName = "Category", outputColumnName = defaultLabelColumn))
                .Append(ctx.Transforms.Text.FeaturizeText(inputColumnName = "Title", outputColumnName = "TitleFeaturized"))
                .Append(ctx.Transforms.Text.FeaturizeText(inputColumnName = "Body", outputColumnName = "BodyFeaturized"))
                .Append(ctx.Transforms.Concatenate(defaultFeaturesColumn, "TitleFeaturized", "BodyFeaturized"))
                .AppendCacheCheckpoint(ctx)

let trainingPipeline = pipeline
                        .Append(ctx.MulticlassClassification.Trainers.SdcaMaximumEntropy(defaultLabelColumn, defaultFeaturesColumn))
                        .Append(ctx.Transforms.Conversion.MapKeyToValue(defaultPredictedLabelColumn))

let trainedModel = trainingPipeline.Fit(trainingData)

let engine = ctx.Model.CreatePredictionEngine<Link, CategoryPrediction>(trainedModel)

//let test = {
//    ID = ""
//    Title = "books"
//    Body = "https://bookshop.com"
//    Category = ""
//}
//
//let prediction = engine.Predict(test)
//
//prediction.Dump()

// let testMetrics = ctx.MulticlassClassification.Evaluate(trainedModel.Transform(testData))

// testMetrics.Dump()

ctx.Model.Save(trainedModel, trainingData.Schema, dataPath + @"\model.zip")
