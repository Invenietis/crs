using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class CommandRunner : ICommandRunner
    {
        IServiceProvider _serviceProvider;
        public CommandRunner( ICommandHandlerFactory factory, IServiceProvider serviceProvider )
        {
            HandlerFactory = factory;
            _serviceProvider = serviceProvider;
        }

        public ICommandHandlerFactory HandlerFactory { get; private set; }

        protected async virtual Task<CommandExecutionContext> DoJob( object command, Type handlerType )
        {
            ICommandHandler handler = HandlerFactory.Create( handlerType );

            CommandReflectionContext reflectionContext = new CommandReflectionContext( command, handlerType);
            CommandExecutionContext exContext = new CommandExecutionContext( _serviceProvider,reflectionContext);

            // TODO: cache
            // TODO: sort of chain of responsibility with a kind of dependency between decorators
            // TODO: ability to dynamicaly register attribute with another IoC
            foreach( var attr in exContext.ReflectionContext.Decorators.OrderBy( a => a.Order ) )
                attr.OnCommandExecuting( exContext );
            try
            {
                exContext.Result = await handler.HandleAsync( command );
            }
            catch( Exception ex )
            {
                exContext.Exception = ex;
            }

            if( exContext.Exception != null )
            {
                foreach( var attr in exContext.ReflectionContext.Decorators.OrderByDescending( a => a.Order ) )
                    attr.OnException( exContext );
            }

            foreach( var attr in exContext.ReflectionContext.Decorators.OrderByDescending( a => a.Order ) )
                attr.OnCommandExecuted( exContext );

            return exContext;
        }

        public virtual async Task<ICommandResponse> RunAsync( CommandProcessingContext ctx, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            CommandExecutionContext result = await DoJob(ctx.Request.Command,  ctx.HandlerType );
            if( result.Exception == null )
            {
                return ctx.CreateDirectResponse( result.Result );
            }
            return ctx.CreateErrorResponse( result.Exception.Message );
        }
    }
}
