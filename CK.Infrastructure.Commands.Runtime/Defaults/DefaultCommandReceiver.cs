using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandReceiver : ICommandReceiver
    {
        readonly Runners _runners;
        readonly ICommandHandlerRegistry _registry;

        public DefaultCommandReceiver( ICommandResponseDispatcher eventDispatcher, ICommandHandlerFactory handlerFactory, ICommandHandlerRegistry registry )
        {
            _registry = registry;
            _runners = new Runners( handlerFactory, eventDispatcher );
        }

        public Task<ICommandResponse> ProcessCommandAsync( ICommandRequest commandRequest, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var context = new CommandProcessingContext( commandRequest, Guid.NewGuid() );
            context.HandlerType = _registry.GetHandlerType( context.Request.CommandDescription.CommandType );
            if( context.HandlerType == null )
            {
                string msg = "Handler not found for command type " + context.Request.CommandDescription.CommandType;
                return Task.FromResult( context.CreateErrorResponse( msg ) );
            }
            return _runners.RunAsync( context, cancellationToken );
        }
    }
}
