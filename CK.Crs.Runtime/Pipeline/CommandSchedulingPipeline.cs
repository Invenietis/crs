using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    public class CommandSchedulingPipeline : IPipeline
    {
        public CommandAction Action { get; }

        public PipelineEvents Events { get; }

        public IActivityMonitor Monitor { get; }

        public CommandRequest Request { get; }

        public CommandResponse Response { get; set; }

        public virtual CancellationToken CancellationToken
        {
            get { return default( CancellationToken ); }
        }
        public IServiceProvider CommandServices { get; }

        public CommandSchedulingPipeline( IServiceProvider serviceProvider, ScheduledCommand command )
        {
            CommandServices = serviceProvider;
            Monitor = command.Token.CreateDependentMonitor();
            Action = command;
            Request = null;
        }
        public void Dispose()
        {
            ((IDisposableActivityMonitor)Monitor).Dispose();
        }
    }
}
