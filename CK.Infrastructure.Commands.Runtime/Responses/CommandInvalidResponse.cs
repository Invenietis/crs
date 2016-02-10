using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class CommandInvalidResponse : CommandResponse
    {
        public CommandInvalidResponse( CommandContext ctx, params ValidationResult[] results ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.ValidationFailed;
            Payload = results;
        }
    }

}
