using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime.Pipeline;

namespace CK.Crs.Runtime
{
    public class CommandReceiver : ICommandReceiver
    {
        readonly PipelineEvents _events;
        readonly IExecutionStrategySelector _executorSelector;
        readonly IFactories _factory;
        readonly IAmbientValues _ambientValues;

        public CommandReceiver( PipelineEvents events, IAmbientValues ambientValues, IExecutionStrategySelector executor, IFactories factory )
        {
            if( events == null ) throw new ArgumentNullException( nameof( events ) );
            if( ambientValues == null ) throw new ArgumentNullException( nameof( ambientValues ) );
            if( executor == null ) throw new ArgumentNullException( nameof( executor ) );
            if( factory == null ) throw new ArgumentNullException( nameof( factory ) );

            _events = events;
            _ambientValues = ambientValues;
            _executorSelector = executor;
            _factory = factory;
        }

        public async Task<CommandResponse> ProcessCommandAsync( ICommandRouteCollection routes, CommandRequest request, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var monitor = new ActivityMonitor( request.Path );

            // Creates the pipeline of the command processing.
            // Each component of the pipeline can set a CommandResponse which will shortcut the command handling. 
            using( var pipeline = new CommandReceivingPipeline( _events, _factory, monitor, request, cancellationToken ) )
            {
                // 1. Looks up the route collection for a registered command.
                await pipeline.RouteCommand( routes );
                // 2. Build the command from the CommandRequest data.
                await pipeline.BuildCommand();
                // 3. Check the ambient values of the command parameters.
                await pipeline.ValidatedAmbientValues( _ambientValues );
                // 4. Invoke command filters. 
                await pipeline.InvokeFilters();
                // 5. Executes the command.
                await pipeline.ExecuteCommand( _executorSelector );

                return pipeline.Response;
            }
        }
    }
}
