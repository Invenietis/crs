using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;
using CK.Crs.Pipeline;

namespace CK.Crs.Runtime
{
    public class CommandReceiver : ICommandReceiver
    {
        readonly IServiceProvider _serviceProvider;
        readonly IServiceScopeFactory _scopeFactory;
        readonly CommandReceiverConfiguration _config;

        public CommandReceiver( IServiceProvider commandReceiverServices, IServiceScopeFactory scopeFactory, CommandReceiverConfiguration config )
        {
            _serviceProvider = commandReceiverServices;
            _scopeFactory = scopeFactory;
            _config = config;
        }

        public async Task ProcessCommandAsync( CommandRequest request, Stream response, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var monitor = new ActivityMonitor( request.Path );
            // Creates the pipeline of the command processing.
            // Each component of the pipeline can set a CommandResponse which will shortcut the command handling. 
            using( var pipeline = new CommandReceivingPipeline( 
                _serviceProvider, 
                _scopeFactory, 
                _config, 
                monitor, 
                request, 
                response,
                cancellationToken ) )
            {
                foreach( var c in _config.Pipeline.Components )
                {
                    await c.Invoke( pipeline );
                }
            }
        }

        class CommandReceivingPipeline : IPipeline, IDisposable
        {
            public IPipelineConfiguration Configuration { get; }

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
                CommandReceiverConfiguration configuration,
                IActivityMonitor monitor,
                CommandRequest request,
                Stream stream,
                CancellationToken cancellationToken )
            {
                _serviceScope = scopeFactory.CreateScope();
                Configuration = configuration;
                Monitor = monitor;
                Request = request;
                Output = stream;
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
