using System.Threading.Tasks;
using CK.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CK.Crs.Infrastructure
{
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = true )]
    internal class MetaProviderAttribute : TypeFilterAttribute
    {
        public MetaProviderAttribute() : base( typeof( CrsMetaProviderImpl ) ) { }

        class CrsMetaProviderImpl : IAsyncActionFilter
        {
            readonly IAmbientValues _ambientValues;
            readonly IAmbientValuesRegistration _registration;
            readonly ICrsModel _model;

            public CrsMetaProviderImpl( IAmbientValues ambientValues, IAmbientValuesRegistration registration, ICrsModel model )
            {
                _ambientValues = ambientValues;
                _registration = registration;
                _model = model;
            }

            public async Task OnActionExecutionAsync( ActionExecutingContext context, ActionExecutionDelegate next )
            {
                if( !context.RouteData.Values.TryGetValue( "action", out object actionTokenValue ) )
                {
                    await next();
                    return;
                }

                if( actionTokenValue == null || actionTokenValue.ToString() != "__meta" )
                {
                    await next();
                    return;
                }

                var commandArgumentName = context.ActionDescriptor.GetProperty<CrsCommandArgumentName>();
                var command = (MetaCommand)context.ActionArguments.GetValueWithDefaultFunc( commandArgumentName, ( k ) => new MetaCommand
                {
                    ShowAmbientValues = true,
                    ShowCommands = true
                } );
                var endpointModel = _model.GetEndpoint( context.Controller.GetType() );
                var response = await MetaCommand.Result.CreateAsync( command, endpointModel, _ambientValues, _registration );
                context.Result =  new OkObjectResult( response );
            }
        }
    }
}
