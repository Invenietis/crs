using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class DirectResponse : CommandResponse
    {
        public DirectResponse( object result, CommandContext ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.Direct;
            Payload = result;
        }
    }
}
