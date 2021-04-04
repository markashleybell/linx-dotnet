<Query Kind="Program">
  <Reference Relative="..\src\app\bin\Debug\net5.0\linx.dll">C:\Src\linx-dotnet\src\app\bin\Debug\net5.0\linx.dll</Reference>
  <Namespace>Linx.Domain</Namespace>
  <Namespace>Microsoft.AspNetCore.Identity</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Linx.Data</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>Linx.Support</Namespace>
  <Namespace>Linx.Services</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

async Task Main()
{
    var settings = new Settings {
        ConnectionString = "Server=localhost;Database=linx;Trusted_Connection=yes",
        SearchIndexBasePath = @"C:\Temp\linx-dotnet"
    };

    var optionsMonitor = new TestOptionsMonitor<Settings>(settings); 
    
    var repository = new SqlServerRepository(optionsMonitor);
    
    var userID = new Guid("e5754cce-838b-4446-ada8-2d5a6e057555");
    
    var (total, pages, links) = await repository.ReadLinksFullAsync(userID, 1, 9999, SortColumn.Created, SortDirection.Descending);
    
    // links.Dump();
    
    var svc = new LuceneSearchService(optionsMonitor);
    
    // svc.DeleteAndRebuildIndex(userID, links);
    
    var linkID = new Guid("a84f61ca-6c71-4682-99ce-ccf9ade1bc75");
    
    var link = await repository.ReadLinkAsync(userID, linkID);
    
    var updated = new Link(linkID, link.Title + " UPDATED", link.Url, link.Abstract + " UPDATED", link.Tags);
    
    // svc.UpdateLink(userID, updated);
    
    // svc.RemoveLink(userID, link);
    
    // svc.AddLink(userID, link);
    
    var results = svc.Search(userID, "data clojure");

    results.Dump();
}

public class TestOptionsMonitor<T> : IOptionsMonitor<T>
    where T : class, new()
{
    public TestOptionsMonitor(T currentValue) =>
        CurrentValue = currentValue;

    public T Get(string name) =>
        CurrentValue;

    public IDisposable OnChange(Action<T, string> listener) =>
        throw new NotImplementedException();

    public T CurrentValue { get; }
}