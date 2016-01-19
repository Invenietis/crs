using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    internal class CommandRunner : ICommandRunner
    {
        private readonly ICommandHandlerFactory _handlerFactory;
        private readonly ICommandDecoratorFactory _commandDecoratorFactory;

        public CommandRunner( ICommandHandlerFactory factory, ICommandDecoratorFactory commandDecoratorFactory )
        {
            _handlerFactory = factory;
            _commandDecoratorFactory = commandDecoratorFactory;
        }

        public async virtual Task RunAsync( CommandExecutionContext exContext, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var mon = exContext.RuntimeContext.Monitor;

            var decorators = exContext.CommandDescription.Decorators.Select( CreateDecorator ).ToArray();
            using( mon.OpenTrace().Send( $"Running Command [{exContext.CommandDescription.CommandType.Name}]..." ) )
            {
                ICommandHandler handler = _handlerFactory.Create( exContext.CommandDescription.HandlerType );
                if( handler == null ) throw new InvalidOperationException( $"Unable to create type {exContext.CommandDescription.HandlerType}" );

                using( mon.OpenTrace().Send( "OnCommandExecuting..." ) )
                {
                    foreach( var attr in decorators.OrderBy( a => a.Order ) )
                    {
                        mon.Trace().Send( $"{attr.GetType().Name}" );
                        attr.OnCommandExecuting( exContext );
                    }
                }
                using( mon.OpenTrace().Send( "Handling..." ) )
                {
                    try
                    {
                        exContext.Result = await handler.HandleAsync( exContext.RuntimeContext );
                    }
                    catch( Exception ex )
                    {
                        exContext.Exception = ex;
                        mon.Error().Send( ex );
                    }
                }

                if( exContext.Exception != null )
                {
                    using( mon.OpenTrace().Send( "OnException..." ) )
                    {
                        foreach( var attr in decorators.OrderByDescending( a => a.Order ) )
                        {
                            mon.Trace().Send( $"{attr.GetType().Name}" );
                            attr.OnException( exContext );
                        }
                    }
                }
                using( mon.OpenTrace().Send( "OnCommandExecuted..." ) )
                {
                    foreach( var attr in decorators.OrderByDescending( a => a.Order ) )
                    {
                        mon.Trace().Send( $"{attr.GetType().Name}" );
                        attr.OnCommandExecuted( exContext );
                    }
                }
            }
        }

        private ICommandDecorator CreateDecorator( Type decoratorType )
        {
            return _commandDecoratorFactory.Create( decoratorType );
        }
    }
}
