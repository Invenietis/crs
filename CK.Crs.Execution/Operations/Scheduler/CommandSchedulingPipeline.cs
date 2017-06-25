using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime.Execution
{
    class CommandSchedulingPipeline : IPipeline, IDisposable
    {
        public CommandAction Action { get; }

        public ICrsConfiguration Configuration { get; }

        public IActivityMonitor Monitor { get; }

        public CommandRequest Request { get; }

        public CommandResponseBuilder Response { get; set; }

        public virtual CancellationToken CancellationToken
        {
            get { return default( CancellationToken ); }
        }

        readonly IServiceScope _scope;
        readonly IDisposable _dependentTokenGroup;

        public IServiceProvider CommandServices { get; }

        public CommandSchedulingPipeline( IServiceProvider serviceProvider, ICrsConfiguration configuration, ScheduledCommand command )
        {
            _scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            CommandServices = _scope.ServiceProvider;
            Configuration = configuration;
            Monitor = new ActivityMonitor();
            _dependentTokenGroup = Monitor.StartDependentActivity( command.Token );

            Action = command;
            Request = null;
        }
        public void Dispose()
        {
            _scope.Dispose();
            _dependentTokenGroup.Dispose();
        }
    }
}
