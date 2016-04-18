using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;

namespace CK.Crs.Pipeline
{
    public class CommandRequest
    {
        public CommandRequest( CommandRoutePath path, Stream body, ClaimsPrincipal user, string callbackIdentifier = null )
        {
            Path = path;
            Body = body;
            User = user;
            CallbackIdentifier = callbackIdentifier;
        }

        /// <summary>
        /// The command request path.
        /// </summary>
        public CommandRoutePath Path { get; }

        /// <summary>
        /// The stream of this request
        /// </summary>
        public Stream Body { get; }

        /// <summary>
        /// The header of this command request
        /// </summary>
        public string CallbackIdentifier { get; }

        /// <summary>
        /// Gets the <see cref="ClaimsPrincipal"/> that issued this command.
        /// </summary>
        public ClaimsPrincipal User { get; }
    }
}
