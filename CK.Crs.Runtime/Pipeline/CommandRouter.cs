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

        public CommandRouter( CommandReceivingPipeline pipeline, ICommandRouteCollection routeCollection ) : base( pipeline )
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
                Pipeline.Request.CommandDescription = _routeCollection.FindCommandDescriptor( Pipeline.Request.Path );
                if( Pipeline.Request.CommandDescription != null )
                {
                    if( Pipeline.Request.CommandDescription.Descriptor.HandlerType == null )
                    {
                        string msg = $"No handler found for command [type={Pipeline.Request.CommandDescription.Descriptor.CommandType}].";
                        Pipeline.Request.Monitor.Error().Send( msg );
                        Pipeline.Response = new CommandInvalidResponse( Pipeline.CommandId, msg );
                    }
                }
            }

            return Task.FromResult( 0 );
        }
    }
}
