using CK.Core;
using CK.Crs.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    [AttributeUsage( AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
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
                var commandContext = context.ActionDescriptor.GetProperty<ICommandContext>();
                var commandArgumentName = context.ActionDescriptor.GetProperty<CrsCommandArgumentName>();

                var obj = context.Filters.SingleOrDefault( t => t.GetType() == typeof( NoAmbientValuesValidationAttribute ) );
                if( obj == null )
                {
                    if( context.ActionArguments.TryGetValue( commandArgumentName.Value, out object commandArgument ) )
                    {
                        var vContext = new ReflectionAmbientValueValidationContext( commandArgument, commandContext.Monitor, _ambientValues );

                        foreach( var v in _registration.AmbientValues )
                        {
                            await vContext.ValidateValueAndRejectOnError( v.Name );
                        }

                        if( vContext.Rejected )
                        {
                            context.Result = new OkObjectResult( new InvalidResponse( commandContext.CommandId, vContext.RejectReason ) );
                            return;
                        }
                    }
                }

                await next();
            }
        }
    }
}