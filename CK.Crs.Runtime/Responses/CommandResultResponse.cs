using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class CommandResultResponse : CommandResponse
    {
        public CommandResultResponse( object result, CommandContext ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.Direct;
            Payload = result;
        }
    }
}
