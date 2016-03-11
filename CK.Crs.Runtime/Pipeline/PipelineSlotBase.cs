using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    abstract class PipelineSlotBase
    {
        protected IPipeline Pipeline;

        protected IActivityMonitor Monitor
        {
            get { return Pipeline.Monitor; }
        }

        public PipelineSlotBase( IPipeline pipeline )
        {
            Pipeline = pipeline;
        }

        public virtual bool ShouldInvoke
        {
            get { return Pipeline.Response == null && Pipeline.Action.Command != null; }
        }

        public abstract Task Invoke( CancellationToken token = default( CancellationToken ) );
    }
}
