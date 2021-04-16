<Query Kind="FSharpProgram">
  <NuGetReference>Microsoft.Data.SqlClient</NuGetReference>
  <NuGetReference>Microsoft.ML</NuGetReference>
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

[<Literal>]
let defaultLabelColumn = "Label"
[<Literal>]
let defaultFeaturesColumn = "Features"
[<Literal>]
let defaultPredictedLabelColumn = "PredictedLabel"

let dataPath = Path.Combine(Path.GetDirectoryName(Util.CurrentQueryPath), "testdata")

[<CLIMutable>]
type Link = {
    [<LoadColumn(0)>] Title : string
    [<LoadColumn(1)>] Abstract : string
    [<LoadColumn(2)>][<ColumnName(defaultPredictedLabelColumn)>] Tags : string
}

let ctx = MLContext(seed = Nullable 0)

let loader = ctx.Data.CreateDatabaseLoader<Link>()

let connectionString = "Server=localhost;Database=linx;Trusted_Connection=yes"

let userId = "e5754cce-838b-4446-ada8-2d5a6e057555";

let sql = sprintf "SELECT Title, Abstract, Tags FROM Links WHERE UserID = '%s'" userId

let dbSource = DatabaseSource(SqlClientFactory.Instance, connectionString, sql);

let data = loader.Load(dbSource);

// data.Preview(10).Dump()

let dataPartitions = ctx.Data.TrainTestSplit(data, testFraction = 0.2)

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

let engine = ctx.Model.CreatePredictionEngine<Link, Link>(trainedModel)

let test = {
    Title = "ASP.NET Core Authentication"
    Abstract = "This talks about authentication and authorisation in .NET Core web applications"
    Tags = ""
}

let prediction = engine.Predict(test)

prediction.Dump()

let testMetrics = ctx.MulticlassClassification.Evaluate(trainedModel.Transform(testData))

testMetrics.Dump()

ctx.Model.Save(trainedModel, trainingData.Schema, dataPath + @"\model.zip")
