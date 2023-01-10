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

    class HttpCrsEndpoint : ICrsEndpoint, IDisposable
    {
        ICommandContext _context;

        public HttpContext HttpContext { get; }

        public HttpCrsEndpoint( HttpContext context )
        {
            HttpContext = context;
        }

        public void Dispose()
        {
            _context?.SetFeature<IHttpContextCommandFeature>( null );
            _context?.SetFeature<IRequestServicesCommandFeature>( null );
        }

        public ICrsEndpointPipeline CreatePipeline( IActivityMonitor monitor, IEndpointModel endpointModel )
        {
            if( _context != null ) throw new InvalidOperationException( "A command context was already set. Please create a new endpoint." );

            if( HttpContext.Request.Path == new PathString( "/" ) || HttpContext.Request.Path == new PathString( "/__meta" ) )
            {
                _context = new MetaCommandContext( monitor, endpointModel, HttpContext.RequestAborted );
            }
            else _context = CreateCommandContext( monitor, endpointModel );

            if( _context != null )
            {
                var httpContextFeature = new HttpContextCommandFeature( HttpContext );
                _context.SetFeature<IHttpContextCommandFeature>( httpContextFeature );
                _context.SetFeature<IRequestServicesCommandFeature>( httpContextFeature );
            }

            return new CrsPipeline( monitor, endpointModel, HttpContext.RequestServices, _context );
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
            if( !commandName.IsValid )
            {
                monitor.Warn( $"CommandName is invalid: {commandName}" );
                return null;
            }

            ICommandRegistry commandRegistry = HttpContext.RequestServices.GetRequiredService<ICommandRegistry>();
            ICommandModel commandModel = commandRegistry.GetCommandByName( commandName );
            if( commandModel == null )
            {
                monitor.Warn( $"Command does not exist: {commandName}" );
                return null;
            }

            commandModel = endpointModel.GetCommandModel( commandModel.CommandType );
            if( commandModel == null )
            {
                monitor.Warn( $"Command was not accepted by this endpoint: {commandName}" );
                return null;
            }

            var callerId = GetCallerId( monitor, endpointModel, HttpContext );
            if( String.IsNullOrEmpty( callerId ) )
            {
                monitor.Warn( $"No CallerId was provided by the client to this endpoint. Please provide one." );
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
            monitor.Trace( $"Looking for \"{endpointModel.CallerIdName}\" in request headers..." );
            if( context.Request.Headers.TryGetValue( endpointModel.CallerIdName, out StringValues callerIdValue ) ) return callerIdValue;

            monitor.Trace( $"Looking for \"{endpointModel.CallerIdName}\" in request query string..." );
            if( context.Request.Query.TryGetValue( endpointModel.CallerIdName, out callerIdValue ) ) return callerIdValue;

            monitor.Warn( $"\"{endpointModel.CallerIdName}\" was not found in either request headers or query string." );
            return callerIdValue;
        }

        static CommandName GetCommandName( HttpContext context )
        {
            return new CommandName( context.Request.Path.Value.Trim( '/' ) );
        }
    }
}
