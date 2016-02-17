using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class CommandInvalidResponse : CommandResponse
    {
        public CommandInvalidResponse( Command ctx, object result ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.ValidationFailed;
            Payload = result;
        }
    }

}
