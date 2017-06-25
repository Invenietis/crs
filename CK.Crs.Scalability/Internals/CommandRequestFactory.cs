using CK.Crs.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;

namespace CK.Crs.Scalability.Internals
{
    /// <summary>
    /// Creates a <see cref="CommandRequest"/> from a <see cref="CommandJob"/>
    /// </summary>
    class DefaultCommandRequestFactory : ICommandRequestFactory
    {
        public virtual CommandRequest CreateFrom(CommandJob message)
        {
            CommandRoutePath path = new CommandRoutePath(message.MetaData.CommandType.Name);

            return new CommandRequest(path, new MemoryStream(message.Payload), ClaimsPrincipal.Current);
        }
    }
}
