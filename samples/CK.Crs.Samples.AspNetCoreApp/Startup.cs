using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CK.Crs.Samples.Messages;
using CK.Core;
using CK.Communication.WebSockets;
using System.IO;
using CK.Monitoring.Handlers;
using CK.Monitoring;
using Rebus.Transport.InMem;
using Rebus.Routing.TypeBased;
using Rebus.Persistence.InMem;
using Rebus.Config;

namespace CK.Crs.Samples.AspNetCoreApp
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices( IServiceCollection services )
        {
            ActivityMonitor.AutoConfiguration = m => m.Output.RegisterUniqueClient<ActivityMonitorConsoleClient>();
            SystemActivityMonitor.RootLogPath = Path.Combine( Directory.GetCurrentDirectory(), "Monitoring" );
            GrandOutput.EnsureActiveDefault( new GrandOutputConfiguration
            {
                Handlers = { new TextFileConfiguration { Path = "Logs" } }
            } );

            var conString = @"Server=.\SQLSERVER2016;Database=RebusQueue;Integrated Security=SSPI";

            services
                .AddCrsCore( config => config
                    .AddCommands( registry => registry.RegisterAssemblies( "CK.Crs.Samples.Handlers" ) )
                    .AddReceivers( endpoints => endpoints
                        .For( typeof( CrsPublicEndpoint<> ) ).AcceptAll()
                        .For( typeof( SimpleCrsEndpoint<> ) ).AcceptAll() ) )
                .AddAmbientValues( ambientValues => ambientValues
                    .AddAmbientValueProviderFrom<MessageBase>()
                        .Select( t => t.ActorId ).ProvidedBy<ActorIdAmbientValueProvider>()
                        .Select( t => t.AuthenticatedActorId ).ProvidedBy<ActorIdAmbientValueProvider>() )
                //.AddBrighter( c => c.DefaultPolicy().NoTaskQueues() )
                .AddRebus( c => c
                        .Transport( t => t.UseSqlServerAsOneWayClient( conString, "tMessages" ) )
                        .Subscriptions( s => s.StoreInSqlServer( conString, "tSubscriptions" ) )
                        .Routing( r => r.TypeBased().MapAssemblyOf<MessageBase>( "command_executor" ) )
                        )
                .AddWebSockets()
                .AddCrsMvcCoreReceiver();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            app.UseDeveloperExceptionPage();
            app.UseWebSockets();
            app.MapWebSockets();
            app.UseMvc();
        }
    }
}
