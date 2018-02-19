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

        public Task InvokeTyped<TCommand>( TCommand command, ICommandContext context )
        {
            if( context == null )
            {
                throw new ArgumentNullException( nameof( context ) );
            }

            var handler = _factory.CreateHandler( context.Model.HandlerType ) as ICommandHandler<TCommand>;
            if( handler == null )
                throw new ArgumentException( String.Format( "Handler {0} for {1} impossible to create.", context.Model.HandlerType, context.Model.CommandType ) );

            try
            {
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

            var handler = _factory.CreateHandler( context.Model.HandlerType ) as ICommandHandler<TCommand, TResult>;
            if( handler == null )
                throw new ArgumentException( String.Format( "Handler {0} for {1} impossible to create.", context.Model.HandlerType, context.Model.CommandType ) );

            try
            {
                return handler.HandleAsync( command, context );
            }
            catch( Exception ex )
            {
                context.Monitor.Error( ex );
                throw;
            }
        }
    }
}
