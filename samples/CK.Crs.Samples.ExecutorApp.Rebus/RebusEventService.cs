using CK.Crs.Samples.Messages;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System;

namespace CK.Crs.Samples.ExecutorApp.Rebus
{
    class RebusEventService : IRebusService
    {
        public void Init( IServiceCollection services )
        {
            var conString = @"Server=.\SQLSERVER2016;Database=RebusQueue;Integrated Security=SSPI";

            services
                .AddCrsCore( c => c
                    .Commands( r => r.RegisterAssemblies( "CK.Crs.Samples.Handlers" ) ) )
                .AddRebusOneWay( c => c
                    .Transport( t => t.UseSqlServer( conString, "tMessages", "events" ) ) );

        }

        public void Start( IServiceProvider services )
        {
        }

        public void Stop( IServiceProvider services )
        {
        }
    }
}
