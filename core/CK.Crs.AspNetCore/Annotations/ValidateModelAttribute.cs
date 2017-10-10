using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace CK.Crs
{
    [AttributeUsage( AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
    public class ValidateModelAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting( ActionExecutingContext context )
        {
            if( !context.ModelState.IsValid )
            {
                var commandContext = context.ActionDescriptor.GetProperty<ICommandContext>();
                context.Result = new OkObjectResult( new InvalidResponse( commandContext.CommandId, context.ModelState ) );
            }
        }

        public void OnActionExecuted( ActionExecutedContext context )
        {
        }
    }
}
