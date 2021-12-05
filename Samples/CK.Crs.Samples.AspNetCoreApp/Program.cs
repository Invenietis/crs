using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace CK.Crs.Samples.AspNetCoreApp
{
    public static class Program
    {
        public static Task Main( string[] args )
        {
            return new HostBuilder().ConfigureWebHostDefaults( webHostBuilder =>
                 webHostBuilder.UseKestrel()
                    .UseContentRoot( Directory.GetCurrentDirectory() )
                    .UseStartup<Startup>()
              )
                .UseCKMonitoring()
                .Build()
                .RunAsync();
        }
    }
}
