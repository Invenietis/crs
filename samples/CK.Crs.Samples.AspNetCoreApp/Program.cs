using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CK.Crs.Samples.AspNetCoreApp
{
    public class Program
    {
        public static void Main( string[] args )
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseMonitoring()
                .UseContentRoot( Directory.GetCurrentDirectory() )
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
