using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Execution
{
    class CommandRunner
    {
        /// <summary>
        /// Invoke the execution pipeline.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static async Task ExecuteAsync( CommandContext context, ICommandHandlerFactory factory )
        {
            var mon = context.ExecutionContext.Monitor;

            var decorators = context.ExecutionContext.Action.Description.Descriptor.Decorators.Select( factory.CreateDecorator ).ToArray();
            using( mon.OpenTrace().Send( $"Running Command [{context.ExecutionContext.Action.Description.Descriptor.CommandType.Name}]..." ) )
            {
                ICommandHandler handler = factory.CreateHandler( context.ExecutionContext.Action.Description.Descriptor.HandlerType );
                if( handler == null ) throw new InvalidOperationException( $"Unable to create type {context.ExecutionContext.Action.Description.Descriptor.HandlerType}" );
                try
                {
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
                            var result = await handler.HandleAsync( context.ExecutionContext, context.ExecutionContext.Action.Command );
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
                finally
                {
                    factory.ReleaseHandler( handler );
                }
            }
        }
    }
}
