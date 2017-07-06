using CK.Crs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{

    /// <summary>
    /// Represent the A of VISAM : Asynchronous response.
    /// </summary>
    public class CommandDeferredResponse : CommandResponse
    {
        public CommandDeferredResponse(Guid id, string callbackId) : base(CommandResponseType.Asynchronous, id)
        {
            Payload = callbackId;
        }
    }

}
