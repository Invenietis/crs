using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CK.Crs.Samples.Messages;
using CK.Core;
using System.IO;
using CK.Monitoring.Handlers;
using CK.Monitoring;
using Rebus.Config;
using CK.Crs.Samples.Handlers;
using Microsoft.Extensions.Configuration;
using CK.Crs.Samples.AspNetCoreApp.Extensions;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Samples.AspNetCoreApp
{
    public class Startup
    {
        public IConfiguration Config { get; }

        public Startup( IHostingEnvironment environment )
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath( environment.ContentRootPath )
                        .AddEnvironmentVariables()
                        .AddJsonFile( "appsettings.json", optional: true, reloadOnChange: true )
                        .AddJsonFile( $"appsettings.{environment.EnvironmentName}.json", reloadOnChange: true, optional: true );

            Config = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            ActivityMonitor.AutoConfiguration = m => m.Output.RegisterUniqueClient<ActivityMonitorConsoleClient>();
            LogFile.RootLogPath = Path.Combine( Directory.GetCurrentDirectory(), "Monitoring" );
            GrandOutput.EnsureActiveDefault( new GrandOutputConfiguration
            {
                Handlers = { new TextFileConfiguration { Path = "Logs" } }
            } );

            var conString = Config.GetConnectionString( "Messaging" );

            services.AddSingleton<IActorIdProvider, Defaults>();
            services.AddSingleton<IUserNameProvider, Defaults>();
            services.AddAmbientValues( a => a.AddProvider<ActorIdAmbientValueProvider>( nameof( MessageBase.ActorId ) ) );

            services
                .AddCrsCore( RegisterCommands )
                .AddRebus(
                    c => c.Transport( t => t.UseSqlServer( conString, "commands_result" ) ),
                    c => c( "commands" )( m => m.HasRebusTag() ) )
                .AddInMemoryReceiver()
                .AddSignalR()
                .AddAspNetCoreHosting();

            services
             .AddCrsCore( r =>
             {
                 // This command is processed synchronously and the result is returned to the caller.
                 // We don't need to specify a result tag etc for this command 
                 r.Register<SyncCommand, SyncCommand.Result, InProcessHandler>();
             } );
        }

        private void RegisterCommands( ICommandRegistry registry )
        {
            // This command is sent to a rebus queue configured below
            registry.Register<RemotelyQueuedCommand>().IsRebus();

            // This command is sent to an in-memory queue executed in webapp process
            registry.Register<QueuedCommand>().HandledBy<InProcessHandler>().FireAndForget();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            app.UseDeveloperExceptionPage();
            app.UseCrs( "/crs", c => c.AcceptAll().SkipAmbientValuesValidation() );
            app.UseCrs( "/crs-admin", c => c
                .FilterCommands( t => t.Tags.Overlaps( c.TraitContext.FindOrCreate( "Admin" ) ) )
                .AddSecurityFilter<PolicySecurityFilter>() );
        }
    }

    class PolicySecurityFilter : ICommandSecurityFilter
    {
        public Task OnFilterAsync( CommandFilterContext filterContext )
        {
            if( filterContext.CommandContext.Model.HasTag( "Security::Policy::MyPolicy" ) )
            {
                // TODO: policy validator.
                bool failed = true;
                if( failed ) filterContext.SetResponse( new Responses.ErrorResponse( "Unauthorized", filterContext.CommandContext.CommandId ) );
            }
            return Task.CompletedTask;
        }
    }
}
