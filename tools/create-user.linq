<Query Kind="Program">
  <Reference Relative="..\src\app\bin\Debug\net5.0\linx.dll">C:\Src\linx-dotnet\src\app\bin\Debug\net5.0\linx.dll</Reference>
  <Reference Relative="..\src\app\bin\Debug\net5.0\Microsoft.Identity.Client.dll">C:\Src\linx-dotnet\src\app\bin\Debug\net5.0\Microsoft.Identity.Client.dll</Reference>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Linx.Domain</Namespace>
  <Namespace>Microsoft.AspNetCore.Identity</Namespace>
  <RuntimeVersion>5.0</RuntimeVersion>
</Query>

void Main()
{
    new PasswordHasher<User>().HashPassword(null, "test123").Dump();
}