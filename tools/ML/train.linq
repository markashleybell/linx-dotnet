<Query Kind="FSharpProgram">
  <Reference Relative="..\..\src\ml\bin\Debug\net5.0\ml.dll">C:\Src\linx-dotnet\src\ml\bin\Debug\net5.0\ml.dll</Reference>
  <NuGetReference>Microsoft.Data.SqlClient</NuGetReference>
  <NuGetReference>Microsoft.ML</NuGetReference>
  <Namespace>LinxML.Common</Namespace>
  <Namespace>Microsoft.Data</Namespace>
  <Namespace>Microsoft.Data.Sql</Namespace>
  <Namespace>Microsoft.Data.SqlClient</Namespace>
  <Namespace>Microsoft.Data.SqlClient.DataClassification</Namespace>
  <Namespace>Microsoft.Data.SqlClient.Server</Namespace>
  <Namespace>Microsoft.Data.SqlTypes</Namespace>
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

let dataSource = "e5754cce-838b-4446-ada8-2d5a6e057555" |> getDataSource

let data = loader.Load(dataSource)

// data.Preview(10).Dump()

let dataPartitions = ctx.Data.TrainTestSplit(data, testFraction = 0.1)

let trainingData = dataPartitions.TrainSet
let testData = dataPartitions.TestSet

let pipeline = EstimatorChain()
                .Append(ctx.Transforms.Conversion.MapValueToKey(inputColumnName = defaultPredictedLabelColumn, outputColumnName = defaultLabelColumn))
                .Append(ctx.Transforms.Text.FeaturizeText(inputColumnName = "Title", outputColumnName = "TitleFeaturized"))
                .Append(ctx.Transforms.Text.FeaturizeText(inputColumnName = "Abstract", outputColumnName = "AbstractFeaturized"))
                .Append(ctx.Transforms.Concatenate(defaultFeaturesColumn, "TitleFeaturized", "AbstractFeaturized"))
                .AppendCacheCheckpoint(ctx)

let trainingPipeline = pipeline
                        .Append(ctx.MulticlassClassification.Trainers.SdcaMaximumEntropy(defaultLabelColumn, defaultFeaturesColumn))
                        .Append(ctx.Transforms.Conversion.MapKeyToValue(defaultPredictedLabelColumn))

let trainedModel = trainingPipeline.Fit(trainingData)

let testMetrics = ctx.MulticlassClassification.Evaluate(trainedModel.Transform(testData))

let results = seq {
    yield { 
        Micro = testMetrics.MicroAccuracy;
        Macro = testMetrics.MacroAccuracy;
        LogLoss = testMetrics.LogLoss;
        LogLossReduction = testMetrics.LogLossReduction; } 
}

results.Dump()

let test = {
    Title = "ASP.NET Core Authentication"
    Abstract = "This talks about authentication and authorisation in .NET Core web applications"
    Tags = ""
}

let engine = ctx.Model.CreatePredictionEngine<Link, Link>(trainedModel)

let prediction = engine.Predict(test)

prediction.Dump()

ctx.Model.Save(trainedModel, trainingData.Schema, modelPath)
