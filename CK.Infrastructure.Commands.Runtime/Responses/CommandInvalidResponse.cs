using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal class CommandInvalidResponse : CommandResponse
    {
        public CommandInvalidResponse( ICollection<ValidationResult> results, CommandContext ctx ) : base( ctx.CommandId )
        {
            ResponseType = CommandResponseType.ValidationFailed;
            Payload = results;
        }
    }

}
