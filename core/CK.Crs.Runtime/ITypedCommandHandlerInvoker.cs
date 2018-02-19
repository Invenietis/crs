using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Typed command handler invoker.
    /// </summary>
    public interface ITypedCommandHandlerInvoker
    {
        /// <summary>
        /// Invokes a typed command.
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="command"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task InvokeTyped<TCommand>( TCommand command, ICommandContext context );

        /// <summary>
        /// Invokes a typed command with a result.
        /// </summary>
        /// <typeparam name="TCommand"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task<TResult> InvokeTypedWithResult<TCommand, TResult>( TCommand command, ICommandContext context );
    }
}
