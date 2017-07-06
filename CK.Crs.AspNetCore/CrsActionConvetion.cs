using CK.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class CrsActionConvention : IActionModelConvention
    {
        public void Apply(ActionModel action)
        {
            if (action.Controller.ControllerType.IsGenericType &&
                ReflectionUtil.IsAssignableToGenericType(
                    action.Controller.ControllerType.GetGenericTypeDefinition(),
                    typeof(ICrsEndpoint<>)))
            {
                action.ActionName = action.Controller.ControllerType.GetGenericArguments()[0].Name;
                action.Filters.Add(new CrsActionFilter());
                action.Filters.Add(new CrsMetaProviderAttribute());
                action.Filters.Add(new ValidateAmbientValuesAttribute());
                action.Filters.Add(new ValidateModelAttribute());
            }
        }

        private class CrsActionFilter : IAsyncActionFilter
        {
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var monitor = EnsureActivityMonitor(context);
                var commandArgumentName = context.ActionDescriptor.Parameters[0].Name;
                context.ActionDescriptor.SetProperty(new CrsCommandArgumentName(commandArgumentName));
                
                using (monitor.OpenTrace().Send("Executing command {0}", context.RouteData.DataTokens["Action"]))
                {
                    await next();
                }
            }

            private static IActivityMonitor EnsureActivityMonitor(ActionExecutingContext context)
            {
                var monitor = context.ActionDescriptor.GetProperty<IActivityMonitor>();
                if (monitor == null)
                {
                    monitor = new ActivityMonitor();
                    context.ActionDescriptor.SetProperty(monitor);
                }
                return monitor;
            }
        }
    }

}
