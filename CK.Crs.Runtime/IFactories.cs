using System;

namespace CK.Crs.Runtime
{
    public interface ICommandFilterFactory
    {
        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="filterType"></param>
        /// <returns></returns>
        ICommandFilter CreateFilter( Type filterType );
    }

    public interface ICommandDecoratorFactory
    {
        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="decoratorType">The type of the decorator to create</param>
        /// <returns>An instance of <see cref="ICommandDecorator"/> or null.</returns>
        ICommandDecorator CreateDecorator( Type decoratorType );
    }

    public interface ICommandHandlerFactory
    {
        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="handlerType">The type of the handler to create.</param>
        /// <returns></returns>
        ICommandHandler CreateHandler( Type handlerType );

        /// <summary>
        /// Releases a <see cref="ICommandHandler"/>.
        /// </summary>
        /// <param name="handler">The command handler to release.</param>
        void ReleaseHandler( ICommandHandler handler );
    }

    public interface ICommandExternalFactory
    {
        /// <summary>
        /// Creates a <see cref="ICommandResponseDispatcher"/> for the current execution context.
        /// </summary>
        /// <returns></returns>
        ICommandResponseDispatcher CreateResponseDispatcher();

        /// <summary>
        /// Creates an instance of <see cref="IExternalEventPublisher"/> used during the execution of the command.
        /// </summary>
        /// <returns></returns>
        IExternalEventPublisher CreateExternalEventPublisher( ICommandExecutionContext context );

        /// <summary>
        /// Creates an instance of <see cref="ICommandScheduler"/>
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        ICommandScheduler CreateCommandScheduler( ICommandExecutionContext context );
    }

    public interface IFactories : ICommandDecoratorFactory, ICommandFilterFactory, ICommandHandlerFactory, ICommandExternalFactory
    {
    }
}
