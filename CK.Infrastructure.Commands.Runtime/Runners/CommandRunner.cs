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

        public async virtual Task RunAsync( CommandExecutionContext exContext, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            ICommandHandler handler = HandlerFactory.Create( exContext.CommandDescription.HandlerType );

            foreach( var attr in exContext.CommandDescription.Decorators.OrderBy( a => a.Order ) ) attr.OnCommandExecuting( exContext );
            try
            {
                exContext.Result = await handler.HandleAsync( exContext.RuntimeContext );
            }
            catch( Exception ex )
            {
                exContext.Exception = ex;
            }

            if( exContext.Exception != null )
                foreach( var attr in exContext.CommandDescription.Decorators.OrderByDescending( a => a.Order ) ) attr.OnException( exContext );

            foreach( var attr in exContext.CommandDescription.Decorators.OrderByDescending( a => a.Order ) ) attr.OnCommandExecuted( exContext );
        }
    }
}
