using CK.Core;
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
        private ICrsModel _model;

        public CrsActionConvention( ICrsModel model )
        {
            _model = model;
        }

        public void Apply( ActionModel action )
        {
            // Only applies on ReceiveCommand action of the Crs Endpoint
            if( action.ActionName != nameof( ICrsEndpoint<object>.ReceiveCommand ) ) return;

            if( action.Controller.ControllerType.IsGenericType &&
                ReflectionUtil.IsAssignableToGenericType(
                    action.Controller.ControllerType.GetGenericTypeDefinition(),
                    typeof( ICrsEndpoint<> ) ) )
            {
                ICrsEndpointModel endpointModel = _model.GetEndpoint( action.Controller.ControllerType );
                if( endpointModel != null )
                {
                    var requestType = action.Controller.ControllerType.GetGenericArguments()[0];
                    var request = endpointModel.GetRequestDescription( requestType );
                    if( request != null )
                    {
                        BasicCrsActionConfiguration( action, request.Name );

                        if( endpointModel.ValidateAmbientValues ) action.Filters.Add( new ValidateAmbientValuesAttribute() );
                        if( endpointModel.ValidateModel ) action.Filters.Add( new ValidateModelAttribute() );
                    }
                    else if ( requestType == typeof( MetaCommand ) )
                    {
                        BasicCrsActionConfiguration( action, "__meta" );
                        action.RouteValues["Action"] = "__meta";
                        action.Filters.Add( new MetaProviderAttribute() );
                    }
                }
            }
        }

        private static void BasicCrsActionConfiguration( ActionModel action, string actionName )
        {
            action.ActionName = actionName;
            action.Parameters[0].BindingInfo = new BindingInfo
            {
                BindingSource = new FromBodyAttribute().BindingSource
            };
            action.Parameters[1].BindingInfo = new ActivityMonitorBindingInfo();
            action.Filters.Add( new CrsActionFilter() );
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
