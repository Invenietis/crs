﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
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
