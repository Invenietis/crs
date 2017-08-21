using CK.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    class DefaultCommandInvoker : ICommandHandlerInvoker
    {
        ICommandHandlerFactory _factory;

        public DefaultCommandInvoker( ICommandHandlerFactory factory )
        {
            _factory = factory ?? throw new ArgumentNullException( nameof( factory ) );
        }

        public async Task<object> Invoke( object command, ICommandContext context )
        {
            if( context.Model.HandlerType == null )
                throw new ArgumentException( String.Format( "No handler registered for {0}", context.Model.Name ) );

            var handler = _factory.CreateHandler( context.Model.HandlerType ) as ICommandHandler;
            if( handler == null )
                throw new ArgumentException( String.Format( "Handler {0} for {1} impossible to created", context.Model.HandlerType ) );

            try
            {
                if( context.Model.ResultType == null )
                {
                    await TaskInvokerVoid( handler, command, context );
                    return null;
                }

                return await TaskInvoker( handler, command, context );
            }
            catch( Exception ex )
            {
                context.Monitor.Error( ex );
                return null;
            }
            finally
            {
                _factory.ReleaseHandler( handler );
            }
        }

        static Task TaskInvokerVoid( ICommandHandler handler, object command, ICommandContext context )
        {
            MethodInfo method = context.Model.HandlerType
              .GetMethod( nameof( ICommandHandler<object>.HandleAsync ), new[] { context.Model.CommandType, typeof( ICommandContext ) } );

            object result = method.Invoke( handler, new object[] { command, context } );

            return (Task)result;
        }

        async static Task<object> TaskInvokerDynamic( ICommandHandler handler, object command, ICommandContext context )
        {
            MethodInfo method = context.Model.HandlerType
                .GetMethod( nameof( ICommandHandler<object>.HandleAsync ), new[] { context.Model.CommandType, typeof( ICommandContext ) } );

            //Change dynamic to var and cast to Task
            dynamic task = method.Invoke( handler, new object[] { command, context } );
            await task;

            //var resultProperty = task.GetType().GetProperty( "Result" );
            //return resultProperty.GetValue( task );
            return task.GetAwaiter().GetResult();

        }

        private static async Task<object> TaskInvoker( ICommandHandler handler, object command, ICommandContext context )
        {
            MethodInfo method = context.Model.HandlerType
                .GetMethod( nameof( ICommandHandler<object>.HandleAsync ), new[] { context.Model.CommandType, typeof( ICommandContext ) } );

            var task = (Task)method.Invoke( handler, new object[] { command, context } );

            await task.ConfigureAwait( false );

            var resultProperty = task.GetType().GetProperty( "Result" );
            return resultProperty.GetValue( task );
        }
    }
}
