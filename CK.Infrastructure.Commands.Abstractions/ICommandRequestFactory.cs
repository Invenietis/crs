using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRequestFactory
    {
        /// <summary>
        /// Creates a <see cref="ICommandRequest"/> from <see cref="CommandRouteRegistration"/> and a body stream payload
        /// </summary>
        /// <param name="routeInfo"></param>
        /// <param name="requestPayload"></param>
        /// <returns></returns>
        ICommandRequest CreateCommand( CommandRouteRegistration routeInfo, Stream requestPayload );
    }
}
