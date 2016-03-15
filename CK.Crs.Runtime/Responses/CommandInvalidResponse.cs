using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class CommandInvalidResponse : CommandResponse
    {
        public CommandInvalidResponse( Guid commandID, string reason ) : base( commandID )
        {
            ResponseType = CommandResponseType.PreConditionFailed;
            Payload = reason;
        }
    }

}
