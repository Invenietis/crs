using CK.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class ValidateAmbientValuesAttribute : TypeFilterAttribute
    {
        public ValidateAmbientValuesAttribute() : base(typeof(AmbientValuesValidationImpl))
        {
        }

        class AmbientValuesValidationImpl : IAsyncActionFilter
        {

            readonly IAmbientValuesRegistration _registration;
            readonly IAmbientValues _ambientValues;
            readonly IMemoryCache _memoryCache;

            public AmbientValuesValidationImpl(IAmbientValuesRegistration registration, IAmbientValues ambientValues, IMemoryCache memoryCache)
            {
                _registration = registration;
                _ambientValues = ambientValues;
                _memoryCache = memoryCache;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var monitor = context.ActionDescriptor.GetProperty<IActivityMonitor>();
                var commandArgumentName = context.ActionDescriptor.GetProperty<CrsCommandArgumentName>();
                var commandArgument = context.ActionArguments[commandArgumentName];

                var vContext = new ReflectionAmbientValueValidationContext(commandArgument, monitor, _ambientValues, _memoryCache);

                foreach (var v in _registration.AmbientValues)
                {
                    await vContext.ValidateValueAndRejectOnError<int>(v.Name);
                }

                if (vContext.Rejected)
                {
                    context.Result = new BadRequestObjectResult(vContext.RejectReason);
                    return;
                }

                await next();
            }
        }
    }
}
