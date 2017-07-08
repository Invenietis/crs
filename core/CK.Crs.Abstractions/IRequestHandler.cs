using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface IRequestHandler<T>
    {
        /// <summary>
        /// Handles a request with the given <see cref="ICommandContext"/>.
        /// </summary>
        /// <remarks>
        /// If the request is like a Command, the context is the one of this command. 
        /// If the request is like an Event, the context is the original command that raises this event.</remarks>
        /// <param name="request">The request to handle.</param>
        /// <param name="context">The command context. See remarks.</param>
        /// <returns></returns>
        Task HandleAsync( T request, ICommandContext context );
    }

}
