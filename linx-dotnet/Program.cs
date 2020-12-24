using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Linx
{
    public static class Program
    {
        public static void Main(string[] args) =>
            Host
                .CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(whb => whb.UseStartup<Startup>())
                .Build()
                .Run();
    }
}
