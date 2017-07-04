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
using Paramore.Brighter;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.Formatters;
using CK.Crs.Samples.AspNetCoreApp.Core;

namespace CK.Crs.Samples.AspNetCoreApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger();

            var configuration = services.AddCommandReceiver(c =>
            {
                // Commands configuration
                c.Commands.Register<SuperCommand>().HandledBy<SuperHandler>();
                c.Commands.Register<Super2Command>().HandledBy<Super2Handler>();

                // Ambient values configuration
                c.AmbientValues
                    .AddAmbientValueProviderFrom<CommandBase>()
                        .Select(t => t.ActorId).ProvidedBy<ActorIdAmbientValueProvider>()
                        .Select(t => t.AuthenticatedActorId).ProvidedBy<ActorIdAmbientValueProvider>();
            });

            // Brighter configuration
            services.AddBrighterExecutor(configuration.Commands, c => c.DefaultPolicy().NoTaskQueues());

            // MVC Core configuration
            services
                .AddMvcCore()
                .AddJsonFormatters()
                .ConfigureApplicationPartManager(p =>
                     p.FeatureProviders.Add(new CrsControllerFeatureProvider(configuration.Commands)));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }
}
