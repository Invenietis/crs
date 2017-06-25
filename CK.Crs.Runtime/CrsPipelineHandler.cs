using System;
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
        readonly IServiceScopeFactory _scopeFactory;
        readonly ICrsConfiguration _config;

        public CrsPipelineHandler( IServiceScopeFactory scopeFactory, ICrsConfiguration config )
        {
            _scopeFactory = scopeFactory;
            _config = config;
        }

        public async Task ProcessCommandAsync( CommandRequest request, CommandResponseBuilder response, IActivityMonitor monitor = null, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            monitor = monitor ?? new ActivityMonitor( request.Path );

            using( var pipeline = new CommandReceivingPipeline( _scopeFactory, _config, monitor, request, response, cancellationToken ) )
            {
                foreach( var c in _config.Pipeline.Components ) await c.Invoke( pipeline );
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

            public CommandResponseBuilder Response { get; set; }
            
            public CancellationToken CancellationToken { get; }

            public IServiceProvider CommandServices
            {
                get { return _serviceScope.ServiceProvider; }
            }

            IDisposableGroup _group;
            IServiceScope _serviceScope;

            public CommandReceivingPipeline(
                IServiceScopeFactory scopeFactory,
                ICrsConfiguration configuration,
                IActivityMonitor monitor,
                CommandRequest request,
                CommandResponseBuilder responseBuilder,
                CancellationToken cancellationToken )
            {
                _serviceScope = scopeFactory.CreateScope();
                Configuration = configuration;
                Monitor = monitor;
                Request = request;
                Response = responseBuilder;
                Action = new CommandAction( Guid.NewGuid() )
                {
                    CallbackId = Request.CallbackIdentifier
                };
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
