using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    class CommandRunner
    {
        private readonly IFactories _factories;

        public CommandRunner( IFactories factories )
        {
            _factories = factories;
        }

        public async virtual Task ExecuteAsync( CommandContext context )
        {
            var mon = context.ExecutionContext.Monitor;

            var decorators = context.Description.Decorators.Select( _factories.CreateDecorator ).ToArray();
            using( mon.OpenTrace().Send( $"Running Command [{context.Description.CommandType.Name}]..." ) )
            {
                ICommandHandler handler = _factories.CreateHandler( context.Description.HandlerType );
                if( handler == null ) throw new InvalidOperationException( $"Unable to create type {context.Description.HandlerType}" );

                using( mon.OpenTrace().Send( "OnCommandExecuting..." ) )
                {
                    foreach( var attr in decorators.OrderBy( a => a.Order ) )
                    {
                        mon.Trace().Send( $"{attr.GetType().Name}" );
                        attr.OnCommandExecuting( context );
                    }
                }
                using( mon.OpenTrace().Send( "Handling..." ) )
                {
                    try
                    {
                        var result = await handler.HandleAsync( context.ExecutionContext, context.ExecutionContext.Model );
                        context.SetResult( result );
                    }
                    catch( Exception ex )
                    {
                        context.SetException( ex );
                        mon.Error().Send( ex );
                    }
                }

                if( context.Exception != null )
                {
                    using( mon.OpenTrace().Send( "OnException..." ) )
                    {
                        foreach( var attr in decorators.OrderByDescending( a => a.Order ) )
                        {
                            mon.Trace().Send( $"{attr.GetType().Name}" );
                            attr.OnException( context );
                        }
                    }
                }
                using( mon.OpenTrace().Send( "OnCommandExecuted..." ) )
                {
                    foreach( var attr in decorators.OrderByDescending( a => a.Order ) )
                    {
                        mon.Trace().Send( $"{attr.GetType().Name}" );
                        attr.OnCommandExecuted( context );
                    }
                }
            }
        }
    }
}
