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
            _factory = factory;
        }


        public async Task<object> Invoke<T>( T command, ICommandContext context, CommandModel desc )
        {
            if( desc == null ) throw new ArgumentException( String.Format( "Command {0} not registered", typeof( T ).Name ) );

            var handler = _factory.CreateHandler( desc.HandlerType ) as ICommandHandler;
            if( handler == null ) throw new ArgumentException( String.Format( "Handler {0} for {1} impossible to created", desc.HandlerType ) );
            try
            {
                MethodInfo methodInfo = desc.HandlerType
                    .GetMethod( nameof( ICommandHandler<object>.HandleAsync ), new[] { desc.CommandType, typeof( ICommandContext ) } );

                if( desc.ResultType == null )
                {
                    await TaskInvokerVoid( methodInfo, handler, command, context, desc );
                    return null;
                }

                return await TaskInvoker( methodInfo, handler, command, context, desc );
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

        static Task TaskInvokerVoid<T>( MethodInfo methodInfo, ICommandHandler handler, T command, ICommandContext context, CommandModel desc )
        {
            object result = methodInfo.Invoke( handler, new object[] { command, context } );

            return (Task)result;
        }

        async static Task<object> TaskInvoker<T>( MethodInfo methodInfo, ICommandHandler handler, T command, ICommandContext context, CommandModel desc )
        {
            //Change dynamic to var and cast to Task
            dynamic task = methodInfo.Invoke( handler, new object[] { command, context } );
            await task;

            //var resultProperty = task.GetType().GetProperty( "Result" );
            //return resultProperty.GetValue( task );
            return task.GetAwaiter().GetResult();

        }
    }
}
