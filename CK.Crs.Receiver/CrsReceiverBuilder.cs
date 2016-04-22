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
        public CrsReceiverBuilder( string routePrefix, IServiceProvider services )
        {
            var registry = services.GetRequiredService<ICommandRegistry>();
            var routeCollection = services.GetRequiredService<CommandRouteCollection>();
            var config = new CrsConfiguration( routePrefix, registry, routeCollection );
            AddConfiguration( config );
        }

        protected override void ConfigureDefaultPipeline( IPipelineBuilder pipeline )
        {
            pipeline
                .UseDefault()
                .UseJsonCommandWriter();
        }
    }
}
