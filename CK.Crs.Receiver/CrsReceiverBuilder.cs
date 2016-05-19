using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    public class CrsReceiverBuilder : CrsHandlerBuilder<CrsConfiguration>
    {
        protected string RoutePrefix { get; private set; }

        protected IServiceProvider ApplicationServices { get; private set; }

        public CrsReceiverBuilder( string routePrefix, IServiceProvider services )
        {
            RoutePrefix = routePrefix;
            ApplicationServices = services;

            var registry = ApplicationServices.GetRequiredService<ICommandRegistry>();
            var routes = new CommandRouteCollection();
            var config = new CrsConfiguration( RoutePrefix, registry, routes );
            AddConfiguration( config );
        }

        protected override void ConfigureDefaultPipeline( ICrsConfiguration configuration )
        {
            configuration.Pipeline.UseDefault();
        }
    }
}
