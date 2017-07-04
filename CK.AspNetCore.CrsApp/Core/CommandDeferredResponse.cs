using CK.Crs.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Samples.AspNetCoreApp.Core
{

    /// <summary>
    /// Represent the A of VISAM : Asynchronous response.
    /// </summary>
    class CommandDeferredResponse : CommandResponse
    {
        public CommandDeferredResponse(Guid id, string callbackId) : base(CommandResponseType.Asynchronous, id)
        {
            Payload = callbackId;
        }
    }

}
