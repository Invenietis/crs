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

            services.AddAmbientValues( a => a.AddAmbientValueProvider<ActorIdAmbientValueProvider>( nameof( MessageBase.ActorId ) ) );

            services
                .AddCrs(
                    c => c.Commands( registry => registry
                        // This command is sent to a rebus queue configured below
                        .Register<RemotelyQueuedCommand>().IsRebus().BroadcastResult()
                        // This command is sent to an in-memory queue executed in webapp process
                        .Register<QueuedCommand>().FireAndForget().HandledBy<InProcessHandler>()
                        // This command is processed synchronously and the result is returned to the caller.
                        // We doesn't need to specify a result tag etc for this command 
                        .Register<SyncCommand>().HandledBy<InProcessHandler>()
                   )
                   .Endpoints( e => e.For( typeof( CrsDispatcherEndpoint<> ) ).AcceptAll() ) )
                .AddRebus(
                    c => c.Transport( t => t.UseSqlServer( conString, "tMessages", "commands_result" ) ),
                    c => c( "commands" )( m => m.HasRebusTag() ) )
                .AddInMemoryReceiver()
                .AddSignalR();
                //.AddClientDispatcher( new ClientDispatcherOptions
                //{
                //    SupportsServerSideEventsFiltering = false
                //} );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
        {
            app.UseDeveloperExceptionPage();
            //app.UseWebSockets();
            //app.MapWebSockets( app.ApplicationServices.GetRequiredService<IOptions<ClientDispatcherOptions>>().Value.WebSocketPath );
            app.UseMvc();
        }
    }
}
