using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.Extensions.Caching.Memory;
using CK.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using CK.Crs.Infrastructure;

namespace CK.Crs
{
    [AttributeUsage( AttributeTargets.Method, AllowMultiple = false, Inherited = true )]
    public class MetaProviderAttribute : TypeFilterAttribute
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

                var result = new MetaCommand.MetaResult();
                var commandArgumentName = context.ActionDescriptor.GetProperty<CrsCommandArgumentName>();
                var command = context.ActionArguments[commandArgumentName] as MetaCommand ?? new MetaCommand
                {
                    ShowAmbientValues = true,
                    ShowCommands = true
                };
                if( command.ShowAmbientValues )
                {
                    result.AmbientValues = new Dictionary<string, object>();
                    foreach( var a in _registration.AmbientValues )
                    {
                        if( _ambientValues.IsDefined( a.Name ) )
                        {
                            var o = await _ambientValues.GetValueAsync( a.Name );
                            result.AmbientValues.Add( a.Name, o );
                        }
                    }
                }
                if( command.ShowCommands )
                {
                    result.Commands = new Dictionary<string, MetaCommand.MetaResult.MetaCommandDescription>();
                    var endpointModel = _model.GetEndpoint( context.Controller.GetType().GetTypeInfo() );
                    foreach( var c in endpointModel.Requests )
                    {
                        MetaCommand.MetaResult.MetaCommandDescription desc = new MetaCommand.MetaResult.MetaCommandDescription
                        {
                            CommandName = c.Name,
                            CommandType = c.Type.AssemblyQualifiedName,
                            Parameters = c.Type.GetTypeInfo().DeclaredProperties.Select( e => new RequestPropertyInfo( e, _registration ) ).ToArray(),
                            Traits = c.Traits?.ToString(),
                            Description = c.Description
                        };
                        result.Commands.Add( desc.CommandName, desc );
                    }
                }
                context.Result = new OkObjectResult( new MetaCommandResponse( result ) );
            }
        }
    }
}
