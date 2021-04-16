namespace LinxML

open Microsoft.Data.SqlClient
open Microsoft.ML
open Microsoft.ML.Data
open System
open System.IO

module Common =

    [<Literal>]
    let defaultLabelColumn = "Label"

    [<Literal>]
    let defaultFeaturesColumn = "Features"

    [<Literal>]
    let defaultPredictedLabelColumn = "PredictedLabel"

    [<CLIMutable>]
    type Link = {
        [<LoadColumn(0)>] Title : string
        [<LoadColumn(1)>] Abstract : string
        [<LoadColumn(2)>] [<ColumnName(defaultPredictedLabelColumn)>] Tags : string }

    type Metric = {
        Micro : float
        Macro : float
        LogLoss : float
        LogLossReduction : float }

    let ctx = MLContext(seed = Nullable 0)

    let loader = ctx.Data.CreateDatabaseLoader<Link>()

    let forValidation (x : IEstimator<_>) =
        match x with
        | :? IEstimator<ITransformer> as y -> y
        | _ -> failwith "Invalid Cast"

    let getDataAndModelPaths (basePath: string) =
        let dataPath = Path.Combine(Path.GetDirectoryName(basePath), "testdata")
        let modelPath = Path.Combine(dataPath, "model.zip")
        (dataPath, modelPath)

    let getDataSource userId =
        let connectionString = "Server=localhost;Database=linx;Trusted_Connection=yes"

        let sql = sprintf "SELECT Title, Abstract, Tags FROM Links WHERE UserID = '%s'" userId

        DatabaseSource(SqlClientFactory.Instance, connectionString, sql)
