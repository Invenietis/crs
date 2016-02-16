using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    internal class CommandResultResponse : CommandResponse
    {
        public CommandResultResponse( object result, Command ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.Direct;
            Payload = result;
        }
    }
}
