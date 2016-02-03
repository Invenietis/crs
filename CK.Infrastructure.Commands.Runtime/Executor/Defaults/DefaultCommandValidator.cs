using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandValidator : ICommandValidator
    {
        public bool TryValidate( CommandExecutionContext executionContext, out ICollection<ValidationResult> results )
        {
            object cmd =  executionContext.RuntimeContext.Command;
            var validationContext = new ValidationContext(cmd ) ;
            results = new List<ValidationResult>();
            
            return Validator.TryValidateObject( cmd, validationContext, results, true );
        }
    }
}
