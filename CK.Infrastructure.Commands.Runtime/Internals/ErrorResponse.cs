using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class ErrorResponse : CommandResponse
    {
        public ErrorResponse( string msg, CommandProcessingContext ctx ) : base( ctx )
        {
            ResponseType = CommandResponseType.Error;
            Payload = msg;
        }
    }

}
