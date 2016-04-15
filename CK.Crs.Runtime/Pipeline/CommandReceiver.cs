using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime.Pipeline
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

        public async Task<CommandResponse> ProcessCommandAsync( CommandRequest request, CancellationToken cancellationToken = default( CancellationToken ) )
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
                cancellationToken ) )
            {
                foreach( var c in _config.Pipeline.Components )
                {
                    await c.Invoke( pipeline );
                }

                return pipeline.Response;
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
                CancellationToken cancellationToken )
            {
                _serviceScope = scopeFactory.CreateScope();
                Configuration = configuration;
                Monitor = monitor;
                Request = request;
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
