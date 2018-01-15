using CK.Core;
using CK.Crs;
using CK.Crs.Meta;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;

namespace CK.Crs.AspNetCore
{

    class HttpCrsEndpoint : ICrsEndpoint
    {
        public HttpContext HttpContext { get; }

        public HttpCrsEndpoint( HttpContext context )
        {
            HttpContext = context;
        }

        public ICrsEndpointPipeline CreatePipeline( IEndpointModel endpointModel )
        {
            IActivityMonitor monitor = new ActivityMonitor();
            ICommandContext context = null;
            if( HttpContext.Request.Path == new PathString( "/" ) || HttpContext.Request.Path == new PathString( "/__meta" ) )
            {
                context = new MetaCommandContext( monitor, endpointModel, HttpContext.RequestAborted );
            }
            else context = CreateCommandContext( monitor, endpointModel );

            if( context != null )
            {
                var httpContextFeature = new HttpContextCommandFeature( HttpContext );
                context.SetFeature<IHttpContextCommandFeature>( httpContextFeature );
                context.SetFeature<IRequestServicesCommandFeature>( httpContextFeature );
            }

            return new HttpCrsEndpointPipeline( monitor, endpointModel, context, HttpContext );
        }

        HttpCommandContext CreateCommandContext( IActivityMonitor monitor, IEndpointModel endpointModel )
        {
            if( monitor == null )
            {
                throw new ArgumentNullException( nameof( monitor ) );
            }

            if( endpointModel == null )
            {
                throw new ArgumentNullException( nameof( endpointModel ) );
            }

            CommandName commandName = GetCommandName( HttpContext );
            if( commandName.IsValid )
            {
                monitor.Warn( $"Receives an invalid command name." );
                return null;
            }

            ICommandRegistry commandRegistry = HttpContext.RequestServices.GetRequiredService<ICommandRegistry>();
            ICommandModel commandModel = commandRegistry.GetCommandByName( commandName );
            if( commandModel == null )
            {
                monitor.Warn( $"Command {commandName} does not exists." );
                return null;
            }

            commandModel = endpointModel.GetCommandModel( commandModel.CommandType );
            if( commandModel == null )
            {
                monitor.Warn( $"Command {commandName} not accepted by this endpoint" );
                return null;
            }

            var callerId = GetCallerId( monitor, endpointModel, HttpContext );
            if( String.IsNullOrEmpty( callerId ) )
            {
                monitor.Warn( $"No caller id provided to this endpoint. Please provides one." );
            }

            var commandId = Guid.NewGuid().ToString( "N" );
            var commandContext = new HttpCommandContext(
                HttpContext,
                commandId,
                monitor,
                commandModel,
                CallerId.Parse( callerId ) );

            return commandContext;
        }

        static string GetCallerId( IActivityMonitor monitor, IEndpointModel endpointModel, HttpContext context )
        {
            monitor.Trace( $"Lookup for {endpointModel.CallerIdName} into the request header" );
            if( context.Request.Headers.TryGetValue( endpointModel.CallerIdName, out StringValues callerIdValue ) ) return callerIdValue;

            monitor.Trace( $"Lookup for {endpointModel.CallerIdName} into the request query string" );
            if( context.Request.Query.TryGetValue( endpointModel.CallerIdName, out callerIdValue ) ) return callerIdValue;

            return callerIdValue;
        }

        static CommandName GetCommandName( HttpContext context )
        {
            return new CommandName( context.Request.Path.Value.Trim( '/' ) );
        }
    }
}
