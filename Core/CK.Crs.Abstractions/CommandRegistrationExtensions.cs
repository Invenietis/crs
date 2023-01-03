using System;

namespace CK.Crs
{
    /// <summary>
    /// Registry extensions
    /// </summary>
    public static class CommandRegistrationExtensions
    {
        /// <summary>
        /// Sets an handler for a command.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="this"></param>
        /// <returns></returns>
        public static ICommandRegistration HandledBy<T>( this ICommandRegistration @this ) where T : ICommandHandler
        {

            if( @this.Model.ResultType == null )
            {
                // ICommandHandler<>
                var commandHandlerInterfaceType = typeof( ICommandHandler<> );
                // ICommandHandler<CommandType>
                commandHandlerInterfaceType = commandHandlerInterfaceType.MakeGenericType( @this.Model.CommandType );
                if( !commandHandlerInterfaceType.IsAssignableFrom( typeof( T ) ) )
                    throw new InvalidOperationException( "This command handler does not match the command model." );
            }
            else
            {
                // ICommandHandler<,>
                var commandHandlerInterfaceType = typeof( ICommandHandler<,> );
                // ICommandHandler<CommandType,ResultType>
                commandHandlerInterfaceType = commandHandlerInterfaceType.MakeGenericType( @this.Model.CommandType, @this.Model.ResultType );
                if( !commandHandlerInterfaceType.IsAssignableFrom( typeof( T ) ) )
                    throw new InvalidOperationException( "This command handler does not match the command model." );
            }

            return @this.Handler( typeof( T ) );
        }

        /// <summary>
        /// Registers a command and its handler.
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static ICommandRegistration Register<TCommand>( this ICommandRegistry registry ) where TCommand : class
        {
            return registry.Register( typeof( TCommand ) );
        }

        /// <summary>
        /// Registers a command with the given handler
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static ICommandRegistration Register<TCommand, THandler>( this ICommandRegistry registry )
            where TCommand : class
            where THandler : ICommandHandler<TCommand>
        {
            return registry.Register<TCommand>().HandledBy<THandler>();
        }

        /// <summary>
        /// Registers a command with the given handler
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="registry"></param>
        /// <returns></returns>
        public static ICommandRegistration Register<TCommand, TResult, THandler>( this ICommandRegistry registry )
            where TCommand : class, ICommand<TResult>
            where THandler : ICommandHandler<TCommand, TResult>
        {
            return registry.RegisterWithResult<TCommand, TResult>().HandledBy<THandler>();
        }

        public static ICommandRegistration RegisterWithResult<TCommand, TResult>( this ICommandRegistry registry )
            where TCommand : ICommand<TResult>
        {
            return registry.Register( typeof( TCommand ), typeof( TResult ) );
        }

        //static ICommandRegistration AddRegistration( ICommandRegistry registry, CommandModel model )
        //{
        //    var registration = new CommandRegistration( registry, model );
        //    registry.Register( model );
        //    if( model.ResultType != null )
        //    {
        //        registration.SetResultTag();
        //    }
        //    return registration;
        //}
    }
}
