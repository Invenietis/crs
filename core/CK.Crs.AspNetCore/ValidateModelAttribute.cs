using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = true )]
    public class ValidateModelAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting( ActionExecutingContext context )
        {
            if( !context.ModelState.IsValid )
            {
                var commandContext = context.ActionDescriptor.GetProperty<IHttpCommandContext>();
                context.Result = new OkObjectResult( new InvalidResponse( commandContext.CommandId, context.ModelState ) );
            }
        }

        public void OnActionExecuted( ActionExecutedContext context )
        {
        }
    }
}
