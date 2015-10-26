using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class DirectResponse : CommandResponse
    {
        public DirectResponse( object result, CommandProcessingContext ctx ) : base( ctx.RuntimeContext.CommandId )
        {
            ResponseType = CommandResponseType.Direct;
            Payload = result;
        }
    }
}
