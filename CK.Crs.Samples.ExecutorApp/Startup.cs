using System;
using CK.Core;
using CK.Crs.Runtime;
using CK.Crs.Samples.Handlers;
using CK.Crs.Samples.Messages;
using CK.Crs.Scalability;
using CK.Crs.Scalability.Azure;
using CK.Crs.Scalability.FileSystem;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Samples.ExecutorApp
{
    class Startup : ICrsStartup
    {
        public Startup()
        {
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IJsonConverter, JsonNetConverter>();

            services.AddCommandRegistration(registry =>
            {
                registry.Register<Super3Command>().HandledBy<Super3Handler>();
            });
            services.AddCommandExecutor();
        }

        public void ConfigureCrs( ICrsConfiguration configuration )
        {
            // Uses default configuration
        }

        public void Configure( ICrsBuilder app)
        {
            app
                .UseFileSystem("D:\\Dev\\vNext\\ck-crs\\FileSystemBus\\");
                //.UseAzure( "storage.azure.net", "ck-crs-samples-messages" );
        }

    }
}