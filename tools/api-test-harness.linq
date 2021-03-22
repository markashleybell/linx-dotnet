<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Net.Http.dll</Reference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json.Serialization</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Net.Http.Headers</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
</Query>

async Task Main()
{
    var httpClient = new HttpClient();

    var uri = new Uri("XXXXXXXXXXXXXXXXX");
    
    httpClient.DefaultRequestHeaders.Add("ApiKey", "XXXXXXXXXXXXXXXXX");

    var emptyFormContent = new FormUrlEncodedContent(Enumerable.Empty<KeyValuePair<string, string>>());

    var postResponse = await httpClient.PostAsync(uri, emptyFormContent);

    // postResponse.Dump();
    
    var responseContent = await postResponse.Content.ReadAsStringAsync();
    
    responseContent.Dump();
}

public static class Extensions 
{
    public static Task<HttpResponseMessage> HeadAsync(this HttpClient httpClient, Uri requestUri)
    {
        return httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, requestUri));
    }
}