<Query Kind="Program">
  <Reference Relative="..\..\microwiki\microwiki\bin\Debug\netcoreapp3.1\microwiki.dll">C:\Src\microwiki\microwiki\bin\Debug\netcoreapp3.1\microwiki.dll</Reference>
  <NuGetReference>Dapper</NuGetReference>
  <Namespace>Dapper</Namespace>
  <Namespace>Microsoft.AspNetCore.Identity</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>MicroWiki</Namespace>
  <Namespace>MicroWiki.Abstract</Namespace>
  <Namespace>MicroWiki.Concrete</Namespace>
  <Namespace>MicroWiki.Controllers</Namespace>
  <Namespace>MicroWiki.Domain</Namespace>
  <Namespace>MicroWiki.Functions</Namespace>
  <Namespace>MicroWiki.Models</Namespace>
  <Namespace>MicroWiki.Support</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

async Task Main()
{
    new PasswordHasher<User>().HashPassword(null, "test123").Dump();
}
