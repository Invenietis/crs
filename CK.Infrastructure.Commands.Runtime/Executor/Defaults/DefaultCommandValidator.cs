using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandValidator : ICommandValidator, ICommandFilter
    {
        public int Order { get; set; }

        public void OnCommandReceived( CommandExecutionContext executionContext )
        {
            object cmd =  executionContext.RuntimeContext.Command;
            var validationContext = new ValidationContext(cmd ) ;
            var results = new List<ValidationResult>();

            if( !Validator.TryValidateObject( cmd, validationContext, results, true ) )
            {
                string resultString = GetString( results);
                executionContext.SetResponse( new CommandErrorResponse( resultString, executionContext.RuntimeContext.CommandId ) );
            }
        }

        private string GetString( List<ValidationResult> results )
        {
            return String.Join( Environment.NewLine, results.Select( x => $"{x.ErrorMessage} [{String.Join( ",", x.MemberNames )}]" ) );
        }
    }
}
