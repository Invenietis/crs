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

namespace CK.AspNetCore.CrsApp
{
    class CommandBase
    {
        public int ActorId { get; set; }
        public int AuthenticatedActorId { get; set; }
    }

    class SuperCommand : CommandBase
    {
    }

    class Super2Command : CommandBase
    {
    }

    class SuperHandler : CommandHandler<SuperCommand>
    {
        protected override Task DoHandleAsync(ICommandExecutionContext context, SuperCommand command)
        {
            Console.WriteLine("Super");
            return Task.CompletedTask;
        }
    }

    class Super2Handler : CommandHandler<Super2Command>
    {
        protected override Task DoHandleAsync(ICommandExecutionContext context, Super2Command command)
        {
            Console.WriteLine("Super Super");
            return Task.CompletedTask;
        }
    }

    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCommandExecutor();
            services.AddCommandReceiver(configuration =>
           {
               configuration.UseJsonNet();

               configuration.Commands.AutoRegisterSimpleCurrentAssembly();
               configuration.Commands
                  .Register<SuperCommand>().HandledBy<SuperHandler>().IsAsync()
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

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
