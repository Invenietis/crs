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
        readonly IPipelineEvents _events;
        readonly ICommandRouteCollection _routes;
        readonly IExecutionStrategySelector _executorSelector;
        readonly IFactories _factory;
        readonly IAmbientValues _ambientValues;

        public CommandReceiver( IPipelineEvents events, IAmbientValues ambientValues, IExecutionStrategySelector executor, ICommandRouteCollection routes, IFactories factory )
        {
            if( events == null ) throw new ArgumentNullException( nameof( events ) );
            if( ambientValues == null ) throw new ArgumentNullException( nameof( ambientValues ) );
            if( executor == null ) throw new ArgumentNullException( nameof( executor ) );
            if( factory == null ) throw new ArgumentNullException( nameof( factory ) );
            if( routes == null ) throw new ArgumentNullException( nameof( routes ) );

            _events = events;
            _ambientValues = ambientValues;
            _executorSelector = executor;
            _factory = factory;
            _routes = routes;
        }

        public async Task<CommandResponse> ProcessCommandAsync( CommandRequest request, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var monitor = new ActivityMonitor( request.Path );
            using( var pipeline = new CommandReceivingPipeline( _factory, monitor, request, cancellationToken ) )
            {
                await pipeline.UseCommandRouter( _routes );
                await pipeline.UseCommandBuilder();
                await pipeline.UseAmbientValuesValidator( _ambientValues );
                await pipeline.UseFiltersInvoker();
                await pipeline.UseCommandExecutor( _executorSelector );
                return pipeline.Response;
            }
        }
    }
}
