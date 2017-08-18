using CK.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
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
            if( action.ActionName != nameof( IHttpCommandReceiver<object>.ReceiveCommand ) ) return;

            if( action.Controller.ControllerType.IsGenericType &&
                ReflectionUtil.IsAssignableToGenericType(
                    action.Controller.ControllerType.GetGenericTypeDefinition(),
                    typeof( IHttpCommandReceiver<> ) ) )
            {
                IEndpointModel endpointModel = _model.GetEndpoint( action.Controller.ControllerType.AsType() );
                if( endpointModel != null )
                {
                    var requestType = action.Controller.ControllerType.GetGenericArguments()[0];
                    var request = endpointModel.GetCommandModel( requestType );
                    if( request != null )
                    {
                        BasicCrsActionConfiguration( action, request.Name );

                        if( endpointModel.ApplyAmbientValuesValidation ) action.Filters.Add( new ValidateAmbientValuesAttribute() );
                        if( endpointModel.ApplyModelValidation ) action.Filters.Add( new ValidateModelAttribute() );
                    }
                    else if( requestType == typeof( MetaCommand ) )
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
            action.RouteValues["Controller"] = action.Controller.ControllerName;
            action.ActionName = actionName;
            action.Parameters[0].BindingInfo = new CommandBindingInfo();
            action.Parameters[1].BindingInfo = new CommandContextBindingInfo();
            //action.Parameters[2].BindingInfo = new HttpContextBindingInfo();
            action.Filters.Add( new CrsActionFilter() );
        }

        private class HttpContextBindingInfo : BindingInfo
        {
            public HttpContextBindingInfo()
            {
                BindingSource = new BindingSource( "HttpContext", "HttpContext", false, false );
                BinderType = typeof( HttpContextModelBinder );
            }

            class HttpContextModelBinder : IModelBinder
            {
                public Task BindModelAsync( ModelBindingContext bindingContext )
                {
                    bindingContext.Result = ModelBindingResult.Success( bindingContext.HttpContext );
                    return Task.CompletedTask;
                }
            }
        }

        private class CommandBindingInfo : BindingInfo
        {
            public CommandBindingInfo()
            {
                BindingSource = new FromBodyAttribute().BindingSource;
            }
        }

        private class CommandContextBindingInfo : BindingInfo
        {
            public CommandContextBindingInfo()
            {
                BinderType = typeof( CommandContextModelBinder );
                BindingSource = new BindingSource( "CommandContext", "CommandContext", true, false );
            }

            private class CommandContextModelBinder : IModelBinder
            {
                ICrsModel _model;
                public CommandContextModelBinder( ICrsModel model )
                {
                    _model = model;
                }

                public Task BindModelAsync( ModelBindingContext bindingContext )
                {
                    // Gets the ControllerType or EndpointName from somehere...
                    var endpointModel = _model.GetEndpointFromContext( bindingContext.ActionContext );
                    if( endpointModel == null ) throw new InvalidOperationException( "There is no endpoint available for this request..." );

                    var commandType = bindingContext.ActionContext.ActionDescriptor.Parameters[0].ParameterType;
                    var commandModel = endpointModel.GetCommandModel( commandType );
                    if( commandModel == null ) throw new InvalidOperationException( "There is no command available for this request..." );

                    var actionName = bindingContext.ActionContext.ActionDescriptor.DisplayName;
                    var monitor = new ActivityMonitor( actionName );

                    // CallerId or ConnectionId provider.
                    var callerValues = bindingContext.ValueProvider.GetValue( endpointModel.CallerIdName );
                    var callerValue = callerValues.FirstValue;
                    if( callerValues.Length > 1 )
                    {
                        monitor.Warn( $"Multiple values found for CallerId={endpointModel.CallerIdName}. Using the first value in the order of ValueProvider registrations: {callerValue}." );
                    }

                    var commandId = Guid.NewGuid(); // TODO: CommandId provider ?

                    var model = new CommandContext(
                        commandId,
                        monitor,
                        commandModel,
                        callerValue,
                        bindingContext.HttpContext.RequestAborted );

                    bindingContext.ActionContext.ActionDescriptor.SetProperty<ICommandContext>( model );
                    bindingContext.Result = ModelBindingResult.Success( model );

                    return Task.CompletedTask;
                }
            }
        }

        private class CrsActionFilter : IAsyncActionFilter
        {
            public async Task OnActionExecutionAsync( ActionExecutingContext context, ActionExecutionDelegate next )
            {
                var commandContext = context.ActionDescriptor.GetProperty<ICommandContext>();
                var commandArgumentName = context.ActionDescriptor.Parameters[0].Name;
                context.ActionDescriptor.SetProperty( new CrsCommandArgumentName( commandArgumentName ) );

                using( commandContext.Monitor.OpenTrace( $"Executing command {commandContext.Model.Name}" ) )
                {
                    await next();
                }
            }
        }
    }
}
