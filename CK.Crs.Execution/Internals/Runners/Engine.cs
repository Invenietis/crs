using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime.Execution
{
    class Engine
    {
        /// <summary>
        /// Invoke the execution pipeline.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static async Task RunAsync( CommandContext context, IExecutionFactory factory )
        {
            var mon = context.ExecutionContext.Monitor;

            var decorators = context.ExecutionContext.Action.Description.Decorators.Select( factory.CreateDecorator ).ToArray();
            using( mon.OpenTrace().Send( $"Running Command [{context.ExecutionContext.Action.Description.CommandType.Name}]..." ) )
            {
                ICommandHandler handler = factory.CreateHandler( context.ExecutionContext.Action.Description.HandlerType );
                if( handler == null ) throw new InvalidOperationException( $"Unable to create type {context.ExecutionContext.Action.Description.HandlerType}" );
                try
                {
                    OnCommandExecuting( context, mon, decorators );
                    await OnCommandHandling( context, mon, handler );
                    OnException( context, mon, decorators );
                    OnCommandExecuted( context, mon, decorators );
                }
                finally
                {
                    factory.ReleaseHandler( handler );
                }
            }
        }

        private static void OnCommandExecuting( CommandContext context, IActivityMonitor mon, ICommandDecorator[] decorators )
        {
            using( mon.OpenTrace().Send( "OnCommandExecuting..." ) )
            {
                foreach( var attr in decorators.OrderBy( a => a.Order ) )
                {
                    mon.Trace().Send( $"{attr.GetType().Name}" );
                    attr.OnCommandExecuting( context );
                }
            }
        }

        private static async Task OnCommandHandling( CommandContext context, IActivityMonitor mon, ICommandHandler handler )
        {
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
        }
        private static void OnCommandExecuted( CommandContext context, IActivityMonitor mon, ICommandDecorator[] decorators )
        {
            using( mon.OpenTrace().Send( "OnCommandExecuted..." ) )
            {
                foreach( var attr in decorators.OrderByDescending( a => a.Order ) )
                {
                    mon.Trace().Send( $"{attr.GetType().Name}" );
                    attr.OnCommandExecuted( context );
                }
            }
        }

        private static void OnException( CommandContext context, IActivityMonitor mon, ICommandDecorator[] decorators )
        {
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
        }

    }
}
