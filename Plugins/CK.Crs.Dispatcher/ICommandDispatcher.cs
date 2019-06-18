using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Dispatchs a <see cref="ICommand"/>.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        /// Dispatches a command.
        /// The command must have been registered in the <see cref="ICommandRegistry"/> before.
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="command"></param>
        /// <param name="commandType"></param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<Response> Send( Guid commandId, object command, Type commandType, CallerId callerId );

        /// <summary>
        /// Dispatches a command.
        /// The command must have been registered in the <see cref="ICommandRegistry"/> before.
        /// </summary>
        /// <param name="commandId"></param>
        /// <param name="command"></param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<Response> Send<TCommand>( Guid commandId, TCommand command, CallerId callerId );

        /// <summary>
        /// Sends a command and waits for the result to be received
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="commandId"></param>
        /// <param name="command"></param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<TResult> SendAndWaitResult<TCommand, TResult>( Guid commandId, TCommand command, CallerId callerId ) where TCommand : ICommand<TResult>;
    }
}
