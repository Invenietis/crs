using CK.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Threading.Tasks;

namespace CK.Crs
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CrsActionConvention : TypeFilterAttribute, IActionModelConvention
    {
        public CrsActionConvention() : base(typeof(CrsAction))
        {
        }

        public void Apply(ActionModel action)
        {
            action.ActionName = action.Controller.ControllerType.GetGenericArguments()[0].Name;
        }

        private class CrsAction : IAsyncActionFilter
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

    internal struct CrsCommandArgumentName
    {
        public CrsCommandArgumentName( string commandName )
        {
            Value = commandName;
        }

        public string Value { get; private set; }

        public static implicit operator string( CrsCommandArgumentName crsCommandName )
        {
            return crsCommandName.Value;
        }

        public static implicit operator CrsCommandArgumentName( string value )
        {
            return new CrsCommandArgumentName(value);
        }
    }

}
