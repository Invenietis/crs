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

        public CommandResponse Response { get; set; }

        public Stream Output { get; set; }

        public virtual CancellationToken CancellationToken
        {
            get { return default( CancellationToken ); }
        }

        IServiceScope _scope;

        public IServiceProvider CommandServices { get; }

        public CommandSchedulingPipeline( IServiceProvider serviceProvider, ICrsConfiguration configuration, ScheduledCommand command )
        {
            _scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();

            CommandServices = _scope.ServiceProvider;
            Configuration = configuration;
            Monitor = command.Token.CreateDependentMonitor();
            Action = command;
            Request = null;
        }
        public void Dispose()
        {
            _scope.Dispose();
            ((IDisposableActivityMonitor)Monitor).Dispose();
        }
    }
}
