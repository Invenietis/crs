using CK.Core;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// A specialization of <see cref="ITypedCommandHandlerInvoker"/>
    /// using <see cref="ICommandHandlerFactory"/> to create command handlers,
    /// and type reflection to find the proper method to call.
    /// </summary>
    public class TypedCommandInvoker : ITypedCommandHandlerInvoker
    {
        readonly ICommandHandlerFactory _factory;

        /// <summary>
        /// Creates a new instance of <see cref="TypedCommandInvoker"/>
        /// </summary>
        /// <param name="factory">The command handler factory to use</param>
        public TypedCommandInvoker( ICommandHandlerFactory factory )
        {
            _factory = factory ?? throw new ArgumentNullException( nameof( factory ) );
        }

        /// <summary>
        /// Attempts to invoke the given command without typing.
        /// </summary>
        /// <param name="command">The CRS command to invoke.</param>
        /// <param name="context">The context of the given CRS command.</param>
        /// <returns>An awaitable task with the object returned by the CRS command handler.</returns>
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

        /// <summary>
        /// Attempts to invoke the given command without a returned result.
        /// </summary>
        /// <param name="command">The CRS command to invoke.</param>
        /// <param name="context">The context of the given CRS command.</param>
        /// <returns>An awaitable task.</returns>
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
                        $"Handler of type {ClassName( context.Model.HandlerType )} must implements ICommandHandler<{ClassName( context.Model.CommandType )}>." );


                return handler.HandleAsync( command, context );
            }
            catch( Exception ex )
            {
                context.Monitor.Error( ex );
                throw;
            }
        }

        /// <summary>
        /// Attempts to invoke the given command with a returned result.
        /// </summary>
        /// <param name="command">The CRS command to invoke.</param>
        /// <param name="context">The context of the given CRS command.</param>
        /// <returns>An awaitable task, with the typed command result.</returns>
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
                        $"Handler of type {ClassName( context.Model.HandlerType )} must implements ICommandHandler<{ClassName( context.Model.CommandType )},{ClassName( context.Model.ResultType )}>." );

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
            Debug.Assert( type.FullName.Remove( 0, type.Namespace.Length + 1 ) == type.Name, "If this is good, ClassName must be replaced with type.Name..." );
            // TODO: Can the line below be replaced with type.Name? (See Assert above)
            return type.FullName.Remove( 0, type.Namespace.Length + 1 );
        }

        protected virtual ICommandHandler CreateHandler( ICommandContext context )
        {
            var handlerObject = _factory.CreateHandler( context.Model.HandlerType );
            if( handlerObject == null )
                throw new ArgumentException( $"Handler of type {context.Model.HandlerType} is impossible to create" );
            return handlerObject;
        }

        static async Task<object> TaskInvoker( ITypedCommandHandlerInvoker invoker, object command, ICommandContext context )
        {
            var genericMethod = invoker.GetType().GetMethod( nameof( ITypedCommandHandlerInvoker.InvokeTypedWithResult ) );
            genericMethod = genericMethod.MakeGenericMethod( context.Model.CommandType, context.Model.ResultType );
            var task = (Task)genericMethod.Invoke( invoker, new[] { command, context } );
            await task.ConfigureAwait( false );

            var resultProperty = task.GetType().GetProperty( "Result" );
            var result = resultProperty.GetValue( task );
            return result;
        }

        static Task TaskInvokerVoid( ITypedCommandHandlerInvoker invoker, object command, ICommandContext context )
        {
            var genericMethod = invoker.GetType().GetMethod( nameof( ITypedCommandHandlerInvoker.InvokeTyped ) );
            genericMethod = genericMethod.MakeGenericMethod( context.Model.CommandType );
            return (Task)genericMethod.Invoke( invoker, new[] { command, context } );
        }
    }
}
