using CK.Core;
using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    class TypedCommandInvoker : ITypedCommandHandlerInvoker
    {
        readonly ICommandHandlerFactory _factory;

        public TypedCommandInvoker( ICommandHandlerFactory factory )
        {
            _factory = factory ?? throw new ArgumentNullException( nameof( factory ) );
        }

        public async Task<object> Invoke( object command, ICommandContext context )
        {
            try
            {
                if( context.Model.ResultType == null )
                {
                    await TaskInvokerVoid( this, command, context ).ConfigureAwait( false );
                    return null;
                }

                return await TaskInvoker( this, command, context ).ConfigureAwait( false );
            }
            catch( Exception ex )
            {
                context.Monitor.Error( ex );
                throw;
            }
        }

        public Task InvokeTyped<TCommand>( TCommand command, ICommandContext context )
        {
            if( context == null )
            {
                throw new ArgumentNullException( nameof( context ) );
            }
            try
            {
                ICommandHandler handlerObject = CreateHandler( context );

                var handler = handlerObject as ICommandHandler<TCommand>;
                if( handler == null )
                    throw new ArgumentException(
                        $"Handler of type {ClassName(context.Model.HandlerType)} must implements ICommandHandler<{ClassName(context.Model.CommandType)}>." );


                return handler.HandleAsync( command, context );
            }
            catch( Exception ex )
            {
                context.Monitor.Error( ex );
                throw;
            }
        }

        public Task<TResult> InvokeTypedWithResult<TCommand, TResult>( TCommand command, ICommandContext context )
        {
            if( context == null )
            {
                throw new ArgumentNullException( nameof( context ) );
            }
            try
            {
                ICommandHandler handlerObject = CreateHandler( context );

                var handler = handlerObject as ICommandHandler<TCommand, TResult>;
                if( handler == null )
                    throw new ArgumentException(
                        $"Handler of type {ClassName(context.Model.HandlerType)} must implements ICommandHandler<{ClassName(context.Model.CommandType)},{ClassName(context.Model.ResultType)}>." );

                context.Monitor.Trace( $"Handler {context.Model.HandlerType} has been succesfuly resolved." );
                return handler.HandleAsync( command, context );
            }
            catch( Exception ex )
            {
                context.Monitor.Error( ex );
                throw;
            }
        }

        private string ClassName( Type type )
        {
            return type.FullName.Remove( 0, type.Namespace.Length + 1 );
        }

        protected virtual ICommandHandler CreateHandler( ICommandContext context )
        {
            var handlerObject = _factory.CreateHandler( context.Model.HandlerType );
            if( handlerObject == null )
                throw new ArgumentException( $"Handler of type {context.Model.HandlerType} is impossible to create" );
            return handlerObject;
        }

        internal static async Task<object> TaskInvoker( ITypedCommandHandlerInvoker invoker, object command, ICommandContext context )
        {
            var genericMethod = invoker.GetType().GetMethod( nameof( ITypedCommandHandlerInvoker.InvokeTypedWithResult ) );
            genericMethod = genericMethod.MakeGenericMethod( context.Model.CommandType, context.Model.ResultType );
            var task = (Task)genericMethod.Invoke( invoker, new[] { command, context } );
            await task.ConfigureAwait( false );

            var resultProperty = task.GetType().GetProperty( "Result" );
            var result = resultProperty.GetValue( task );
            return result;
        }

        internal static Task TaskInvokerVoid( ITypedCommandHandlerInvoker invoker, object command, ICommandContext context )
        {
            var genericMethod = invoker.GetType().GetMethod( nameof( ITypedCommandHandlerInvoker.InvokeTyped ) );
            genericMethod = genericMethod.MakeGenericMethod( context.Model.CommandType );
            return (Task)genericMethod.Invoke( invoker, new[] { command, context } );
        }
    }
}
