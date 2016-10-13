using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;

namespace CK.Crs.Runtime.Routing
{
    class CommandRouter : PipelineComponent
    {
        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response == null;
        }

        public override Task Invoke( IPipeline pipeline, CancellationToken token )
        {
            var routeData =  pipeline.Configuration.Routes.FindRoute( pipeline.Configuration.ReceiverPath, pipeline.Request.Path );
            if( routeData != null )
            {
                pipeline.Action.Description = routeData.Descriptor;
                if( pipeline.Action.Description.HandlerType == null )
                {
                    string msg = $"No handler found for command [type={pipeline.Action.Description.CommandType}].";
                    pipeline.Monitor.Error().Send( msg );
                    pipeline.Response = new CommandInvalidResponse( pipeline.Action.CommandId, msg );
                }
            }
            else
            {
                string msg = $"No routes found for {pipeline.Request.Path} in Receiver={pipeline.Configuration.ReceiverPath}";
                pipeline.Monitor.Info().Send( msg );
#if DEBUG
                using( pipeline.Monitor.OpenInfo().Send( "Registered routes are:" ) )
                {
                    foreach( var r in pipeline.Configuration.Routes.All )
                    {
                        pipeline.Monitor.Trace().Send( r.Route.ToString() );
                    }
                }
#endif                    
            }
            return Task.FromResult( 0 );
        }
    }
}
