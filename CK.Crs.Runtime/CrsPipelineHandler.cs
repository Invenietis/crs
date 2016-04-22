using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using CK.Crs.Runtime;

namespace CK.Crs.Runtime
{
    class CrsPipelineHandler : ICrsHandler
    {
        readonly IServiceProvider _serviceProvider;
        readonly IServiceScopeFactory _scopeFactory;
        readonly ICrsConfiguration _config;

        public CrsPipelineHandler( IServiceProvider commandReceiverServices, IServiceScopeFactory scopeFactory, ICrsConfiguration config )
        {
            _serviceProvider = commandReceiverServices;
            _scopeFactory = scopeFactory;
            _config = config;
        }

        public async Task<bool> ProcessCommandAsync( CommandRequest request, Stream response, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var monitor = new ActivityMonitor( request.Path );

            using( var pipeline = new CommandReceivingPipeline( _serviceProvider, _scopeFactory, _config, monitor, request, response, cancellationToken ) )
            {
                foreach( var c in _config.Pipeline.Components ) await c.Invoke( pipeline );
                return pipeline.Response != null;
            }
        }

        class CommandReceivingPipeline : IPipeline, IDisposable
        {
            public ICrsConfiguration Configuration { get; }

            public CommandRequest Request { get; }

            public CommandAction Action { get; }
            /// <summary>
            /// Gets the <see cref="IActivityMonitor"/>
            /// </summary>
            public IActivityMonitor Monitor { get; }

            public CommandResponse Response { get; set; }

            public Stream Output { get; }

            public CancellationToken CancellationToken { get; }

            public IServiceProvider CommandServices
            {
                get { return _serviceScope.ServiceProvider; }
            }

            IDisposableGroup _group;
            IServiceScope _serviceScope;

            public CommandReceivingPipeline(
                IServiceProvider commandReceiverServices,
                IServiceScopeFactory scopeFactory,
                ICrsConfiguration configuration,
                IActivityMonitor monitor,
                CommandRequest request,
                Stream outputStream,
                CancellationToken cancellationToken )
            {
                _serviceScope = scopeFactory.CreateScope();
                Configuration = configuration;
                Monitor = monitor;
                Request = request;
                Output = outputStream;
                Action = new CommandAction( Guid.NewGuid() );
                CancellationToken = cancellationToken;
                _group = Monitor.OpenTrace().Send( "Invoking command receiver Pipeline..." );
            }

            public void Dispose()
            {
                _group.Dispose();
                _serviceScope.Dispose();
            }
        }
    }
}
