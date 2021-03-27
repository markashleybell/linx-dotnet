<Query Kind="Program">
  <Reference Relative="..\src\app\bin\Debug\net5.0\linx.dll">C:\Src\linx-dotnet\src\app\bin\Debug\net5.0\linx.dll</Reference>
  <Namespace>Linx.Domain</Namespace>
  <Namespace>Microsoft.AspNetCore.Identity</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Linx.Data</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>Linx.Support</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

async Task Main()
{
    var settings = new Settings {
        ConnectionString = "Server=localhost;Database=linx;Trusted_Connection=yes"
    };

    var optionsMonitor = new TestOptionsMonitor<Settings>(settings); 
    
    var repository = new SqlServerRepository(optionsMonitor);
    
    var userID = new Guid("e5754cce-838b-4446-ada8-2d5a6e057555");
    
    var r = await repository.CheckIfLinkExistsByUrlPrefix(userID, "https://www.typescriptlang.org/docs/handbook/utility-types.html");
    
    r.Dump();
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