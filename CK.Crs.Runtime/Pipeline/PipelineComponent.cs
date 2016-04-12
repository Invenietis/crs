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
        public IPipeline Pipeline { get; }

        protected IActivityMonitor Monitor
        {
            get { return Pipeline.Monitor; }
        }

        public PipelineComponent( IPipeline pipeline )
        {
            Pipeline = pipeline;
        }

        public abstract bool ShouldInvoke
        {
            get;
        }

        public async Task<IPipeline> TryInvoke( CancellationToken token = default( CancellationToken ) )
        {
            if( ShouldInvoke ) await Invoke( token );
            return Pipeline;
        }

        public abstract Task Invoke( CancellationToken token = default( CancellationToken ) );
    }
}
