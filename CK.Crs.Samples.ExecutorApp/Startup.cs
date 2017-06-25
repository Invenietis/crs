using System;
using CK.Core;
using CK.Crs.Runtime;
using CK.Crs.Samples.Handlers;
using CK.Crs.Samples.Messages;
using CK.Crs.Scalability;
using CK.Crs.Scalability.Azure;
using CK.Crs.Scalability.FileSystem;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Samples.ExecutorApp
{
    class Startup : ICrsStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.Add(ServiceDescriptor.Singleton<IJsonConverter, JsonNetConverter>());

            services.AddCommandRegistration(registry =>
            {
                registry.Register<Super3Command>().HandledBy<Super3Handler>();
            });
            services.AddCommandExecutor(configuration =>
            {
                //configuration.CommandRunningStore = new DistributedCommandRunningStore();
            });

        }

        public void ConfigurePipeline( IPipelineBuilder pipeline )
        {
            pipeline
                .UseJsonCommandBuilder()
                .UseSyncCommandExecutor()
                .UseJsonCommandWriter();
        }

        public void Configure( ICrsBuilder app)
        {
            app
                .UseFileSystem( "//SharedDrive/CommandsJobs" )
                .UseAzure( "storage.azure.net", "ck-crs-samples-messages" );
        }

    }
}