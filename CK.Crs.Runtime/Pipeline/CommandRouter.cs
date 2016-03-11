using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class CommandRouter : PipelineSlotBase
    {
        readonly ICommandRouteCollection _routeCollection;

        public CommandRouter( IPipeline pipeline, ICommandRouteCollection routeCollection ) : base( pipeline )
        {
            _routeCollection = routeCollection;
        }

        public override bool ShouldInvoke
        {
            get { return Pipeline.Response == null; }
        }

        public override Task Invoke( CancellationToken token )
        {
            if( ShouldInvoke )
            {
                Pipeline.Action.Description = _routeCollection.FindCommandDescriptor( Pipeline.Request.Path );
                if( Pipeline.Action.Description != null )
                {
                    if( Pipeline.Action.Description.Descriptor.HandlerType == null )
                    {
                        string msg = $"No handler found for command [type={Pipeline.Action.Description.Descriptor.CommandType}].";
                        Pipeline.Monitor.Error().Send( msg );
                        Pipeline.Response = new CommandInvalidResponse( Pipeline.Action.CommandId, msg );
                    }
                }
            }

            return Task.FromResult( 0 );
        }
    }
}
