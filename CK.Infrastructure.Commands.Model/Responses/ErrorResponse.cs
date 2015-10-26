using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class ErrorResponse : CommandResponse
    {
        public ErrorResponse( string msg, Guid commandId ) : base( commandId )
        {
            ResponseType = CommandResponseType.Error;
            Payload = msg;
        }
    }

}
