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

        public Task OnCommandReceived( ICommandFilterContext filterContext )
        {
            var validationContext = new ValidationContext(filterContext.Command ) ;
            var results = new List<ValidationResult>();

            if( !Validator.TryValidateObject( filterContext.Command, validationContext, results, true ) )
            {
                string resultString = GetString( results);
                filterContext.Reject( resultString );
            }

            return Task.FromResult<object>( null );
        }

        private string GetString( List<ValidationResult> results )
        {
            return String.Join( Environment.NewLine, results.Select( x => $"{x.ErrorMessage} [{String.Join( ",", x.MemberNames )}]" ) );
        }
    }
}
