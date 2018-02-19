using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    public static class CommandInvokerExtensions
    {
        public static async Task<object> InvokeGeneric( this ITypedCommandHandlerInvoker invoker, object command, ICommandContext context )
        {
            if( context.Model.ResultType == null )
            {
                await TaskInvokerVoid( invoker, command, context );
                return null;
            }

            return await TaskInvoker( invoker, command, context );
        }

        private static async Task<object> TaskInvoker( ITypedCommandHandlerInvoker invoker, object command, ICommandContext context )
        {
            var genericMethod = invoker.GetType().GetMethod( nameof( ITypedCommandHandlerInvoker.InvokeTypedWithResult ) );
            genericMethod = genericMethod.MakeGenericMethod( context.Model.CommandType, context.Model.ResultType );
            var task = (Task)genericMethod.Invoke( invoker, new[] { command, context } );
            await task.ConfigureAwait( false );

            var resultProperty = task.GetType().GetProperty( "Result" );
            var result = resultProperty.GetValue( task );
            return result;
        }

        private static Task TaskInvokerVoid( ITypedCommandHandlerInvoker invoker, object command, ICommandContext context )
        {
            var genericMethod = invoker.GetType().GetMethod( nameof( ITypedCommandHandlerInvoker.InvokeTyped ) );
            genericMethod = genericMethod.MakeGenericMethod( context.Model.CommandType );
            return (Task)genericMethod.Invoke( invoker, new[] { command, context } );
        }
    }
}
