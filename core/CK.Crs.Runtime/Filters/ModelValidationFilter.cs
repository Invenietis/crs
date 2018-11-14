using CK.Core;
using CK.Crs.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class ModelValidationFilter : ICommandFilter
    {
        public Task OnFilterAsync( CommandFilterContext filterContext )
        {
            using( filterContext.Monitor.OpenTrace( "Validating command model" ) )
            {
                var validationResults = new List<ValidationResult>();

                var serviceProvider = filterContext.CommandContext.GetRequestServices();
                var validationContext = new ValidationContext( filterContext.Command, serviceProvider, items: null );
                if( !Validator.TryValidateObject( filterContext.Command, validationContext, validationResults, validateAllProperties: true ) )
                {
                    filterContext.Monitor.Info( "Validation failed" );
                    filterContext.SetResponse( new InvalidResponse( filterContext.CommandContext.CommandId, validationResults ) );
                }
            }

            return Task.CompletedTask;
        }
    }
}
