<Query Kind="Program">
  <Reference Relative="..\src\app\bin\Debug\net5.0\linx.dll">C:\Src\linx-dotnet\src\app\bin\Debug\net5.0\linx.dll</Reference>
  <NuGetReference Prerelease="true">Lucene.Net</NuGetReference>
  <NuGetReference Prerelease="true">Lucene.Net.Analysis.Common</NuGetReference>
  <Namespace>Linx.Data</Namespace>
  <Namespace>Linx.Domain</Namespace>
  <Namespace>Linx.Support</Namespace>
  <Namespace>Lucene.Net.Analysis</Namespace>
  <Namespace>Lucene.Net.Index</Namespace>
  <Namespace>Lucene.Net.Store</Namespace>
  <Namespace>Lucene.Net.Util</Namespace>
  <Namespace>Microsoft.AspNetCore.Identity</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Lucene.Net.Analysis.Standard</Namespace>
  <Namespace>Lucene.Net.Documents</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

async Task Main()
{
    const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

    var indexPath = @"C:\Temp\linx-dotnet\test-index";

    using var dir = FSDirectory.Open(indexPath);

    var analyzer = new StandardAnalyzer(AppLuceneVersion);

    var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer) {
        OpenMode = OpenMode.CREATE
    };
    
    using var writer = new IndexWriter(dir, indexConfig);
    
    var sources = new[] {
        new {
            Name = "Cat",
            Description = "This is a cat."
        },
        new {
            Name = "Dog",
            Description = "This is a dog."
        },
        new {
            Name = "Catfish",
            Description = "This is a fish, that looks like a cat."
        },
        new {
            Name = "Dogfish",
            Description = "This is a fish, that looks like a dog."
        }
    };
    
    var docs = sources.Select(s => new Document {
        new StringField("name", s.Name, Field.Store.YES),
        new TextField("description", s.Description, Field.Store.YES)
    });

    writer.AddDocuments(docs);
    writer.Flush(triggerMerge: false, applyAllDeletes: false);
}
