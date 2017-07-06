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
using CK.Core;

namespace CK.Crs.Samples.AspNetCoreApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCrs( config => config
                .AddAmbientValues( ambientValues =>
                {
                    ambientValues.AddAmbientValueProviderFrom<CommandBase>()
                        .Select(t => t.ActorId).ProvidedBy<ActorIdAmbientValueProvider>()
                        .Select(t => t.AuthenticatedActorId).ProvidedBy<ActorIdAmbientValueProvider>();
                })
                .AddCommands( registry =>
                {
                    registry
                        .Register<SuperCommand>().HandledBy<SuperHandler>()
                        .Register<Super2Command>().HandledBy<Super2Handler>();
                })
                .AddEndpoints( endpoints => 
                {
                    endpoints
                        .For(typeof(CrsAdminEndpoint<>)).Apply(command => command.CommandType.Namespace.EndsWith("Admin"))
                        .For(typeof(CrsPublicEndpoint<>)).Apply(command => !command.CommandType.Namespace.EndsWith("Admin"));
                })
           );
            //.AddBrighterExecutor( c => c.DefaultPolicy().NoTaskQueues() );


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }
}
