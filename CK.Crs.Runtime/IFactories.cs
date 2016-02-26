using System;

namespace CK.Crs.Runtime
{
    public interface IFactories
    {
        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        ICommandHandler CreateHandler( Type handlerType );

        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        ICommandDecorator CreateDecorator( Type decoratorType );

        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="handlerType"></param>
        /// <returns></returns>
        ICommandFilter CreateFilter( Type filterType );

        ICommandResponseDispatcher CreateResponseDispatcher();

        /// <summary>
        /// Creates an instance of <see cref="IExternalEventPublisher"/> used during the execution of the command.
        /// </summary>
        /// <returns></returns>
        IExternalEventPublisher CreateExternalEventPublisher( ICommandExecutionContext context );
    }
}
