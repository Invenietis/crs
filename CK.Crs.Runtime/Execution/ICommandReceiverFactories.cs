using System;

namespace CK.Crs
{
    public interface ICommandReceiverFactories
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
    }
}
