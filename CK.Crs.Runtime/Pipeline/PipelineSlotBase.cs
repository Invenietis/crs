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
        protected CommandReceivingPipeline Pipeline;

        protected IActivityMonitor Monitor
        {
            get { return Pipeline.Request.Monitor; }
        }

        public PipelineSlotBase( CommandReceivingPipeline pipeline )
        {
            Pipeline = pipeline;
        }

        public virtual bool ShouldInvoke
        {
            get { return Pipeline.Response == null && Pipeline.Request.Command != null; }
        }

        public abstract Task Invoke( CancellationToken token = default( CancellationToken ) );
    }
}
