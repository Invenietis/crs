﻿using CK.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Infrastructure
{
    class CrsActionConvention : IActionModelConvention
    {
        public void Apply( ActionModel action )
        {
            if( action.Controller.ControllerType.IsGenericType &&
                ReflectionUtil.IsAssignableToGenericType(
                    action.Controller.ControllerType.GetGenericTypeDefinition(),
                    typeof( ICrsEndpoint<> ) ) )
            {
                action.ActionName = action.Controller.ControllerType.GetGenericArguments()[0].Name;
                action.Parameters[0].BindingInfo = new BindingInfo
                {
                    BindingSource = new FromBodyAttribute().BindingSource
                };
                action.Parameters[1].BindingInfo = new ActivityMonitorBindingInfo();
                action.Filters.Add( new CrsActionFilter() );
                action.Filters.Add( new MetaProviderAttribute() );
                action.Filters.Add( new ValidateAmbientValuesAttribute() );
                action.Filters.Add( new ValidateModelAttribute() );
            }
        }

        private class ActivityMonitorModelBinder : IModelBinder
        {
            public Task BindModelAsync( ModelBindingContext bindingContext )
            {
                var actioName = bindingContext.ActionContext.ActionDescriptor.DisplayName;
                var monitor = new ActivityMonitor( actioName );
                bindingContext.ActionContext.ActionDescriptor.SetProperty<IActivityMonitor>( monitor );
                bindingContext.IsTopLevelObject = true;
                bindingContext.Result = ModelBindingResult.Success( monitor );
                return Task.CompletedTask;
            }
        }

        private class CrsActionFilter : IAsyncActionFilter
        {
            public async Task OnActionExecutionAsync( ActionExecutingContext context, ActionExecutionDelegate next )
            {
                var monitor = EnsureActivityMonitor( context );
                var commandArgumentName = context.ActionDescriptor.Parameters[0].Name;
                context.ActionDescriptor.SetProperty( new CrsCommandArgumentName( commandArgumentName ) );

                using( monitor.OpenTrace( $"Executing command {context.RouteData.Values["action"]}" ) )
                {
                    await next();
                }
            }

            private static IActivityMonitor EnsureActivityMonitor( ActionExecutingContext context )
            {
                return context.ActionDescriptor.GetProperty<IActivityMonitor>() ?? new ActivityMonitor();
            }
        }

        private class ActivityMonitorBindingInfo : BindingInfo
        {
            public ActivityMonitorBindingInfo()
            {
                BinderType = typeof( ActivityMonitorModelBinder );
                BindingSource = new BindingSource( "ActivityMonitor", "ActivityMonitor", true, false );
            }
        }
    }

}
