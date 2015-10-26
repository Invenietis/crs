using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRequestFactory
    {
        /// <summary>
        /// Creates a <see cref="ICommandRequest"/> from <see cref="CommandDescriptor"/> and a body stream payload
        /// </summary>
        /// <param name="routeInfo"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ICommandRequest> CreateCommand( CommandDescriptor routeInfo, HttpRequest request, CancellationToken cancellationToken = default( CancellationToken ) );
    }
}
