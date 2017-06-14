using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime
{
    public abstract class CrsReceiverBuilder : CrsHandlerBuilder<CrsConfiguration>
    {
        protected string RoutePrefix { get; private set; }

        protected ICommandRegistry Registry { get; private set; }

        public CrsReceiverBuilder( string routePrefix, ICommandRegistry registry )
        {
            RoutePrefix = routePrefix;
            Registry = registry;

            var routes = new CommandRouteCollection();
            var config = new CrsConfiguration( RoutePrefix, Registry, routes );
            AddConfiguration( config );
        }
    }
}
