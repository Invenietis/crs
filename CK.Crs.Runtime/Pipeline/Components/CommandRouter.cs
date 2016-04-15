using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class CommandRouter : PipelineComponent
    {
        public CommandRouter()
        {
        }

        public override bool ShouldInvoke( IPipeline pipeline )
        {
            return pipeline.Response == null;
        }

        public override Task Invoke( IPipeline pipeline, CancellationToken token )
        {
            pipeline.Action.Description = pipeline.Configuration.Routes.FindCommandDescriptor( pipeline.Request.Path );
            if( pipeline.Action.Description != null )
            {
                if( pipeline.Action.Description.Descriptor.HandlerType == null )
                {
                    string msg = $"No handler found for command [type={pipeline.Action.Description.Descriptor.CommandType}].";
                    pipeline.Monitor.Error().Send( msg );
                    pipeline.Response = new CommandInvalidResponse( pipeline.Action.CommandId, msg );
                }
            }
            else
            {
                string msg = $"No routes found for {pipeline.Request.Path} in Receiver={pipeline.Configuration.Routes.PathBase}";
                pipeline.Monitor.Info().Send( msg );
#if DEBUG
                using( pipeline.Monitor.OpenInfo().Send( "Registered routes are:" ) )
                {
                    var implicitRouteCollection = pipeline.Configuration.Routes as CommandRouteCollection;
                    if( implicitRouteCollection != null )
                        foreach( var r in implicitRouteCollection.RouteStorage )
                        {
                            pipeline.Monitor.Trace().Send( r.Key.ToString() );
                        }

                }
#endif                    
            }
            return Task.FromResult( 0 );
        }
    }
}
