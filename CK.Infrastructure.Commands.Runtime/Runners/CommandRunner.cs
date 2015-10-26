using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class CommandRunner : ICommandRunner
    {
        public CommandRunner( ICommandHandlerFactory factory )
        {
            HandlerFactory = factory;
        }

        public ICommandHandlerFactory HandlerFactory { get; private set; }

        protected async virtual Task<CommandExecutionContext> DoJob( CommandProcessingContext processingContext, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            ICommandHandler handler = HandlerFactory.Create( processingContext.Request.CommandDescription.HandlerType );

            CommandExecutionContext exContext = new CommandExecutionContext( processingContext.RuntimeContext );

            // TODO: cache
            // TODO: sort of chain of responsibility with a kind of dependency between decorators
            // TODO: ability to dynamicaly register attribute with another IoC
            foreach( var attr in processingContext.Request.CommandDescription.Decorators.OrderBy( a => a.Order ) )
                attr.OnCommandExecuting( exContext );
            try
            {
                exContext.Result = await handler.HandleAsync( processingContext.RuntimeContext );
            }
            catch( Exception ex )
            {
                exContext.Exception = ex;
            }

            if( exContext.Exception != null )
            {
                foreach( var attr in processingContext.Request.CommandDescription.Decorators.OrderByDescending( a => a.Order ) )
                    attr.OnException( exContext );
            }

            foreach( var attr in processingContext.Request.CommandDescription.Decorators.OrderByDescending( a => a.Order ) )
                attr.OnCommandExecuted( exContext );

            return exContext;
        }

        public virtual async Task<ICommandResponse> RunAsync( CommandProcessingContext ctx, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            CommandExecutionContext result = await DoJob(ctx, cancellationToken );
            if( result.Exception == null )
            {
                return ctx.CreateDirectResponse( result.Result );
            }
            return ctx.CreateErrorResponse( result.Exception.Message );
        }
    }
}
