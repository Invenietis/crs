using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Infrastructure.Commands
{
    public abstract class CommandExecutor : ICommandExecutor
    {
        private readonly ICommandReceiverFactories _factories;

        public CommandExecutor( ICommandReceiverFactories factories )
        {
            _factories = factories;
        }

        public async virtual Task ExecuteAsync( CommandExecutionContext exContext, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            var mon = exContext.RuntimeContext.Monitor;

            var decorators = exContext.CommandDescription.Decorators.Select( _factories.CreateDecorator ).ToArray();
            using( mon.OpenTrace().Send( $"Running Command [{exContext.CommandDescription.CommandType.Name}]..." ) )
            {
                ICommandHandler handler = _factories.CreateHandler( exContext.CommandDescription.HandlerType );
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
                        var result = await handler.HandleAsync( exContext.RuntimeContext );
                        var response = new CommandResultResponse( result, exContext.RuntimeContext);
                        exContext.SetResponse( response );
                    }
                    catch( Exception ex )
                    {
                        exContext.SetException( ex );
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
    }
}
