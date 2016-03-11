using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    internal class CommandResultResponse : CommandResponse
    {
        public CommandResultResponse( object result, CommandExecutionContext ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.Direct;
            Payload = result;
        }
    }
}
