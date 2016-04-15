using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    public abstract class PipelineComponent
    {
        public abstract bool ShouldInvoke( IPipeline pipeline );

        public async Task<IPipeline> TryInvoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) )
        {
            if( ShouldInvoke( pipeline ) ) await Invoke( pipeline );
            else
            {
                pipeline.Monitor.Warn().Send( "Component {0} should not been invoked. Condition failed.", GetType().Name );
            }
            return pipeline;
        }

        public abstract Task Invoke( IPipeline pipeline, CancellationToken token = default( CancellationToken ) );
    }
}
