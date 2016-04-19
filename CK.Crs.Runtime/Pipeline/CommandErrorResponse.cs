﻿using System;

namespace CK.Crs.Runtime
{
    public class CommandErrorResponse : CommandResponse
    {
        public CommandErrorResponse( string msg, Guid commandId ) : base( commandId )
        {
            ResponseType = CommandResponseType.Error;
            Payload = msg;
        }
    }

}
