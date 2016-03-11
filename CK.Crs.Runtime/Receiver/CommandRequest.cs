using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    public class CommandRequest 
    {
        public CommandRequest( CommandRoutePath path, Stream body, ClaimsPrincipal user, IActivityMonitor monitor )
        {
            Path = path;
            Body = body;
            User = user;
            Monitor = monitor;
        }

        /// <summary>
        /// The command request path.
        /// </summary>
        public CommandRoutePath Path { get; }

        /// <summary>
        /// The stream of this request
        /// </summary>
        public Stream Body { get;  }

        /// <summary>
        /// Gets the <see cref="IActivityMonitor"/>
        /// </summary>
        public IActivityMonitor Monitor { get; }

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> that issued this command.
        /// </summary>
        public ClaimsPrincipal User { get; }

        /// <summary>
        /// The instance of the command to process. This should never be null.
        /// </summary>
        public object Command { get; set; }

        /// <summary>
        /// Command description
        /// </summary>
        public RoutedCommandDescriptor CommandDescription { get; set; }

        /// <summary>
        /// Returns an identifier that should help identifying the caller of this request.
        /// </summary>
        public string CallbackId { get; set; }

    }
}
