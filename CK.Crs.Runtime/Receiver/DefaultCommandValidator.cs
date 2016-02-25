using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class DefaultCommandValidator : ICommandFilter
    {
        public int Order { get; set; }

        public Task OnCommandReceived( CommandContext context )
        {
            object cmd =  context.ExecutionContext.Model;
            var validationContext = new ValidationContext(cmd ) ;
            var results = new List<ValidationResult>();

            if( !Validator.TryValidateObject( cmd, validationContext, results, true ) )
            {
                string resultString = GetString( results);
                context.SetResult( new ValidationResult( resultString ) );
            }

            return Task.FromResult<object>( null );
        }

        private string GetString( List<ValidationResult> results )
        {
            return String.Join( Environment.NewLine, results.Select( x => $"{x.ErrorMessage} [{String.Join( ",", x.MemberNames )}]" ) );
        }
    }
}
