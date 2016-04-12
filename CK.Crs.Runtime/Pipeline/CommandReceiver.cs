using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    public class CommandReceiver : ICommandReceiver
    {
        readonly IServiceProvider _serviceProvider;
        readonly IPipelineBuilder _pipelineConfiguration;
        readonly PipelineEvents _events;
        readonly ICommandRouteCollection _routes;

        public CommandReceiver( IServiceProvider serviceProvider, IPipelineBuilder pipelineConfiguration, PipelineEvents events, ICommandRouteCollection routes )
        {
            _serviceProvider = serviceProvider;
            _pipelineConfiguration = pipelineConfiguration;
            _events = events;
            _routes = routes;
        }

        public async Task<CommandResponse> ProcessCommandAsync( CommandRequest request, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var monitor = new ActivityMonitor( request.Path );
            // Creates the pipeline of the command processing.
            // Each component of the pipeline can set a CommandResponse which will shortcut the command handling. 
            using( var pipeline = new CommandReceivingPipeline( _serviceProvider, _events, _routes, monitor, request, cancellationToken ) )
            {
                foreach( var c in _pipelineConfiguration.Components ) await c.Invoke( pipeline );

                return pipeline.Response;
            }
        }

        class CommandReceivingPipeline : IPipeline, IDisposable
        {
            public PipelineEvents Events { get; }

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
                get { return _container; }
            }

            IDisposableGroup _group;
            SimpleServiceContainer _container;

            public CommandReceivingPipeline( IServiceProvider serviceProvider, PipelineEvents events, ICommandRouteCollection routes, IActivityMonitor monitor, CommandRequest request, CancellationToken cancellationToken )
            {
                _container = new SimpleServiceContainer( serviceProvider );
                _container.Add( typeof( ICommandRouteCollection ), routes, null );
                _container.Add( typeof( PipelineEvents ), events, null );
                _container.Add( typeof( IActivityMonitor ), monitor, null );

                Events = events;
                Monitor = monitor;
                Request = request;
                Action = new CommandAction( Guid.NewGuid() );
                CancellationToken = cancellationToken;
                _group = Monitor.OpenTrace().Send( $"Processing new command [commandId={Action.CommandId}]..." );
            }

            public void Dispose()
            {
                _group.Dispose();
                _container.Dispose();
            }
        }
    }
}
