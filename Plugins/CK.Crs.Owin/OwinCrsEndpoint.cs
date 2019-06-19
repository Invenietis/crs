using CK.Core;
using CK.Crs.Meta;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Owin;
using System;
using System.Linq;
using System.Threading;

namespace CK.Crs.Owin
{

    class OwinCrsEndpoint : ICrsEndpoint, IDisposable
    {
        private readonly IServiceScope _serviceScope;
        ICommandContext _context;

        public IOwinContext OwinContext { get; }
        public IServiceProvider Services { get; }

        public OwinCrsEndpoint( IOwinContext context, IServiceProvider applicationServices, bool scopeServices = true )
        {
            if( applicationServices == null )
            {
                throw new ArgumentNullException( nameof( applicationServices ) );
            }

            OwinContext = context ?? throw new ArgumentNullException( nameof( context ) );
            if( scopeServices )
            {
                _serviceScope = OwinContext.Get<IServiceScope>( nameof( IServiceScope ) );
                if( _serviceScope == null )
                {
                    var serviceScopeFactory = applicationServices.GetRequiredService<IServiceScopeFactory>();
                    _serviceScope = serviceScopeFactory.CreateScope();
                }

                Services = _serviceScope.ServiceProvider;
            }
            else
            {
                Services = applicationServices;
            }
        }

        public ICrsEndpointPipeline CreatePipeline( IActivityMonitor monitor, IEndpointModel endpointModel )
        {
            if( _context != null ) throw new InvalidOperationException( "There is already a command context. Please creates a new endpoint." );

            if( OwinContext.Request.Path == new PathString( "/" ) || OwinContext.Request.Path == new PathString( "/__meta" ) )
            {
                _context = new MetaCommandContext( monitor, endpointModel, CancellationToken.None );
            }
            else _context = CreateCommandContext( monitor, endpointModel );

            if( _context != null )
            {
                var owinContextFeature = new OwinContextCommandFeature( OwinContext, Services );
                _context.SetFeature<IOwinContextCommandFeature>( owinContextFeature );
                _context.SetFeature<IRequestServicesCommandFeature>( owinContextFeature );
            }

            return new CrsPipeline( monitor, endpointModel, Services, _context );
        }

        OwinCommandContext CreateCommandContext( IActivityMonitor monitor, IEndpointModel endpointModel )
        {
            if( monitor == null )
            {
                throw new ArgumentNullException( nameof( monitor ) );
            }

            if( endpointModel == null )
            {
                throw new ArgumentNullException( nameof( endpointModel ) );
            }

            CommandName commandName = GetCommandName( OwinContext );
            if( !commandName.IsValid )
            {
                monitor.Warn( $"Receives an invalid command name." );
                return null;
            }

            ICommandRegistry commandRegistry = Services.GetRequiredService<ICommandRegistry>();
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

            var callerId = GetCallerId( monitor, endpointModel, OwinContext );
            if( String.IsNullOrEmpty( callerId ) )
            {
                monitor.Warn( $"No caller id provided to this endpoint. Please provides one." );
            }

            var commandId = Guid.NewGuid().ToString( "N" );
            var commandContext = new OwinCommandContext(
                OwinContext,
                commandId,
                monitor,
                commandModel,
                CallerId.Parse( callerId ) );

            return commandContext;
        }

        static string GetCallerId( IActivityMonitor monitor, IEndpointModel endpointModel, IOwinContext context )
        {
            monitor.Trace( $"Lookup for {endpointModel.CallerIdName} into the request header" );
            if( context.Request.Headers.TryGetValue( endpointModel.CallerIdName, out string[] callerIdValues ) ) return callerIdValues.FirstOrDefault();

            monitor.Trace( $"Lookup for {endpointModel.CallerIdName} into the request query string" );

            var callerIdValue = context.Request.Query
                .FirstOrDefault( x => x.Key.Equals( endpointModel.CallerIdName, StringComparison.InvariantCultureIgnoreCase ) )
                .Value?.FirstOrDefault();

            return callerIdValue;
        }

        static CommandName GetCommandName( IOwinContext context )
        {
            return new CommandName( context.Request.Path.Value.Trim( '/' ) );
        }

        public void Dispose()
        {
            _serviceScope?.Dispose();
            _context.SetFeature<IOwinContextCommandFeature>( null );
            _context.SetFeature<IRequestServicesCommandFeature>( null );
        }
    }
}
