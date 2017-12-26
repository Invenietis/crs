using CK.Core;
using CK.Crs.Responses;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace CK.Crs.AspNetCore
{
    public class ModelValidationFilter : ICrsFilter
    {
        public Task ApplyFilterAsync( CrsFilterContext filterContext )
        {
            if( filterContext.Model.ApplyModelValidation )
            {
                using( filterContext.Monitor.OpenTrace( "Validating command model" ) )
                {
                    var items = new Dictionary<object, object>();
                    var validationResults = new List<ValidationResult>();
                    var validationContext = new ValidationContext( filterContext.Command, filterContext.HttpContext.RequestServices, items );
                    if( !Validator.TryValidateObject( filterContext.Command, validationContext, validationResults ) )
                    {
                        filterContext.Monitor.Info( "Validation failed" );
                        filterContext.SetResponse( new InvalidResponse( filterContext.Context.CommandId, validationResults ) );
                    }
                }
            }
            else filterContext.Monitor.Trace( "Command model validation skipped by endpoint configuration" );

            return Task.CompletedTask;
        }
    }
}
