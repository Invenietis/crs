using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Pipeline;

namespace CK.Crs.Runtime.Execution
{
    public class CommandSchedulingPipeline : IPipeline, IDisposable
    {
        public CommandAction Action { get; }

        public IPipelineConfiguration Configuration { get; }

        public IActivityMonitor Monitor { get; }

        public CommandRequest Request { get; }

        public CommandResponse Response { get; set; }

        public Stream Output { get; set; }

        public virtual CancellationToken CancellationToken
        {
            get { return default( CancellationToken ); }
        }
        public IServiceProvider CommandServices { get; }

        public CommandSchedulingPipeline( IServiceProvider serviceProvider, IPipelineConfiguration configuration, ScheduledCommand command )
        {
            CommandServices = serviceProvider;
            Configuration = configuration;
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
