using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CK.Crs;
using System.Reflection;
using CK.Crs.Samples.Messages;
using CK.Crs.Samples.Handlers;
using CK.Crs.Scalability.FileSystem;

namespace CK.Crs.Samples.AspNetCoreApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger();
            services.AddCommandExecutor();
            services.AddCommandReceiver(configuration =>
           {
               configuration.UseJsonNet();
               
               configuration.Commands.AutoRegisterSimpleCurrentAssembly();
               configuration.Commands
                  .Register<SuperCommand>().IsAsync()
                  .Register<Super3Command>().IsScalable()
                  .Register<Super2Command>().HandledBy<Super2Handler>();

               configuration.AmbientValues
                    .AddAmbientValueProviderFrom<CommandBase>()
                        .Select(t => t.ActorId).ProvidedBy<ActorIdAmbientValueProvider>()
                        .Select(t => t.AuthenticatedActorId).ProvidedBy<ActorIdAmbientValueProvider>();

           });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCrsWithDefault();

            app.UseCrs("/commands", config =>
            {
                config.Pipeline
                    .Clear()
                    .UseMetaComponent( config.Routes )
                    .UseSwagger( config.Routes )
                    .UseCommandRouter(config.Routes)
                    .UseJsonCommandBuilder()
                    .UseAmbientValuesValidator()
                    .UseFilters( config.Routes )
                    .UseFileSystemCommandBus( config.TraitContext, new FileSystemConfiguration("//SharedDrive/CommandsJobs/Inputs") )
                    .UseTaskBasedCommandExecutor(config.TraitContext)
                    .UseSyncCommandExecutor()
                    .UseJsonCommandWriter();
            });
            
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
