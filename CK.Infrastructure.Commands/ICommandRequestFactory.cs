using Microsoft.AspNet.Http;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRequestFactory
    {
        /// <summary>
        /// Creates a <see cref="ICommandRequest"/> from <see cref="CommandRouteRegistration"/> and a body stream payload
        /// </summary>
        /// <param name="routeInfo"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        ICommandRequest CreateCommand( CommandRouteRegistration routeInfo, HttpRequest request );
    }
}
