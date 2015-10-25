using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class DeferredResponse : CommandResponse
    {
        public DeferredResponse( string callbackId, CommandProcessingContext ctx ) : base( ctx )
        {
            ResponseType = CommandResponseType.Deferred;
            Payload = callbackId;
        }
    }
}
