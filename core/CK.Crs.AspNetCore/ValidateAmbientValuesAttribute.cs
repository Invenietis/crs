using CK.Core;
using CK.Crs.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class ValidateAmbientValuesAttribute : TypeFilterAttribute
    {
        public ValidateAmbientValuesAttribute() : base( typeof( AmbientValuesValidationImpl ) )
        {
        }

        class AmbientValuesValidationImpl : IAsyncActionFilter
        {
            readonly IAmbientValuesRegistration _registration;
            readonly IAmbientValues _ambientValues;

            public AmbientValuesValidationImpl( IAmbientValuesRegistration registration, IAmbientValues ambientValues )
            {
                _registration = registration;
                _ambientValues = ambientValues;
            }

            public async Task OnActionExecutionAsync( ActionExecutingContext context, ActionExecutionDelegate next )
            {
                var monitor = context.ActionDescriptor.GetProperty<IActivityMonitor>();
                var commandArgumentName = context.ActionDescriptor.GetProperty<CrsCommandArgumentName>();
                var commandArgument = context.ActionArguments[commandArgumentName];

                var obj = context.Filters.SingleOrDefault( t => t.GetType() == typeof( NoAmbientValuesValidationAttribute ) );
                if( obj == null )
                {
                    var vContext = new ReflectionAmbientValueValidationContext( commandArgument, monitor, _ambientValues );

                    foreach( var v in _registration.AmbientValues )
                    {
                        await vContext.ValidateValueAndRejectOnError( v.Name );
                    }

                    if( vContext.Rejected )
                    {
                        context.Result = new BadRequestObjectResult( vContext.RejectReason );
                        return;
                    }
                }

                await next();
            }
        }
    }
}
