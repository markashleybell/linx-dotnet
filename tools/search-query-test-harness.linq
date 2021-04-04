<Query Kind="Program">
  <Reference Relative="..\src\app\bin\Debug\net5.0\linx.dll">C:\Src\linx-dotnet\src\app\bin\Debug\net5.0\linx.dll</Reference>
  <NuGetReference Prerelease="true">Lucene.Net</NuGetReference>
  <NuGetReference Prerelease="true">Lucene.Net.Analysis.Common</NuGetReference>
  <NuGetReference Prerelease="true">Lucene.Net.QueryParser</NuGetReference>
  <Namespace>Linx.Data</Namespace>
  <Namespace>Linx.Domain</Namespace>
  <Namespace>Linx.Support</Namespace>
  <Namespace>Lucene.Net.Analysis</Namespace>
  <Namespace>Lucene.Net.Analysis.Standard</Namespace>
  <Namespace>Lucene.Net.Documents</Namespace>
  <Namespace>Lucene.Net.Index</Namespace>
  <Namespace>Lucene.Net.Search</Namespace>
  <Namespace>Lucene.Net.Store</Namespace>
  <Namespace>Lucene.Net.Util</Namespace>
  <Namespace>Microsoft.AspNetCore.Identity</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Lucene.Net.QueryParsers.Classic</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

async Task Main()
{
    const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

    var indexPath = @"C:\Temp\linx-dotnet\test-index";

    using var dir = FSDirectory.Open(indexPath);

    using IndexReader reader = DirectoryReader.Open(dir);
    
    var searcher = new IndexSearcher(reader);
    var analyzer = new StandardAnalyzer(AppLuceneVersion);

    var parser = new QueryParser(AppLuceneVersion, "description", analyzer);
    
    var parser2 = new MultiFieldQueryParser(AppLuceneVersion, new[] { "name", "description" }, analyzer);
    
    var phrase = new MultiPhraseQuery
        {
            new Term("description", "brown"),
            new Term("description", "fox")
        };
        
    var query = parser2.Parse("fish dog");
    
    var hits = searcher.Search(query, 20 /* top 20 */).ScoreDocs;

    var results = hits.Select(h => {
        var doc = searcher.Doc(h.Doc);
        return new {
            Score = h.Score,
            Doc = new { Name = doc.Get("name"), Description = doc.Get("description") }
        };
    });

    results.OrderByDescending(r => r.Score).Dump();
}
