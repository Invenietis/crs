
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommand<TResult> { }

    // 1. ICommand<TGenerique -> Result>
    // 2. NestedClass Result
    // 3. Special type VoidResult.

    public interface ICommandHandler
    {
    }

    public interface ICommandHandler<T> : ICommandHandler
    {
        /// <summary>
        /// Handles a request with the given <see cref="ICommandContext"/>.
        /// </summary>
        /// <param name="command">The request to handle.</param>
        /// <param name="context">The command context.</param>
        /// <returns></returns>
        Task HandleAsync( T command, ICommandContext context );
    }

    public interface ICommandHandler<in T, TResult> : ICommandHandler
    {
        /// <summary>
        /// Handles a request with the given <see cref="ICommandContext"/>.
        /// </summary>
        /// <param name="command">The request to handle.</param>
        /// <param name="context">The command context.</param>
        /// <returns></returns>
        Task<TResult> HandleAsync( T command, ICommandContext context );
    }
}
