using CK.Crs.Samples.Handlers;
using CK.Crs.Samples.Messages;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System;

namespace CK.Crs.Samples.ExecutorApp.Rebus
{
    class RebusCommandService : IRebusService
    {
        public void Init( IServiceCollection services )
        {
            var conString = @"Server=.\SQLSERVER2016;Database=RebusQueue;Integrated Security=SSPI";

            services
                .AddCrsCore(
                    c => c.Commands( registry => registry
                        .Register<SuperCommand>().HandledBy<SuperHandler>()
                        .Register<Super2Command>().HandledBy<Super2Handler>()
                        .Register<SuperCommand.Result>().IsRebusQueue() ) )
                .AddRebusOneWay(
                    c => c.Transport( t => t.UseSqlServer( conString, "tMessages", "commands" ) ),
                    c => c( "commands_result" )( m => m.HasRebusQueueTag() ) );

        }

        public void Start( IServiceProvider services )
        {
        }

        public void Stop( IServiceProvider services )
        {
        }
    }
}
