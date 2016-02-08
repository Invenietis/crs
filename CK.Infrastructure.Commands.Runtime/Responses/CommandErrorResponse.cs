using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class CommandErrorResponse : CommandResponse
    {
        public CommandErrorResponse( string msg, Guid commandId ) : base( commandId )
        {
            ResponseType = CommandResponseType.Error;
            Payload = msg;
        }
    }

}
