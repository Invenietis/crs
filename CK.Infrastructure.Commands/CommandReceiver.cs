using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Infrastructure.Commands
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    public class CommandReceiver
    {
        private readonly ICommandRegistry _registry;
        private readonly RequestDelegate _next;

        public CommandReceiver( RequestDelegate next, ICommandRegistry registry )
        {
            _next = next;
            _registry = registry;
        }

        public async Task Invoke( HttpContext httpContext )
        {
            var commandRegistration = _registry.Find( httpContext.Request.Path );
            if( commandRegistration == null && _next != null )
            {
                await _next( httpContext );
                return;
            }

            ICommandRequestFactory commandRequestFactory = ResolveCommandFactory( httpContext );
            ICommandRequest commandRequest = await commandRequestFactory.CreateCommand( commandRegistration, httpContext.Request, httpContext.RequestAborted );
            if( commandRequest == null )
            {
                string msg = String.Format( "A valid command definition has been infered from routes, but the command type {0} failed to be instanciated.", commandRegistration.CommandType.Name );
                throw new InvalidOperationException( msg );
            }

            ICommandReceiver commandReceiver = ResolveCommandReceiver( httpContext );
            ICommandResponse commandResponse = await commandReceiver.ProcessCommandAsync( commandRequest, httpContext.RequestAborted );
            if( commandResponse == null )
            {
                string msg = String.Format( "A valid command response must be received" );
                throw new InvalidOperationException( msg );
            }

            ICommandResponseSerializer responseSerializer = ResolveCommandResponseSerializer( httpContext );
            responseSerializer.Serialize( commandResponse, httpContext.Response.Body );
        }

        #region Resolvers

        protected virtual ICommandRequestFactory ResolveCommandFactory( HttpContext context )
        {
            ICommandRequestFactory commandFactory = context.ApplicationServices.GetService<ICommandRequestFactory>();
            if( commandFactory == null )
            {
                throw new InvalidOperationException( "An implementation of ICommandRequestFactory must be registered into the ApplicationServices" );
            }
            return commandFactory;
        }

        protected virtual ICommandReceiver ResolveCommandReceiver( HttpContext context )
        {
            ICommandReceiver o = context.ApplicationServices.GetService<ICommandReceiver>();
            if( o == null )
            {
                throw new InvalidOperationException( "An implementation of ICommandReceiver must be registered into the ApplicationServices" );
            }
            return o;
        }

        protected virtual ICommandResponseSerializer ResolveCommandResponseSerializer( HttpContext context )
        {
            ICommandResponseSerializer o = context.ApplicationServices.GetService<ICommandResponseSerializer>();
            if( o == null )
            {
                throw new InvalidOperationException( "An implementation of ICommandResponseSerializer must be registered into the ApplicationServices" );
            }
            return o;
        }

        #endregion

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CommandReceiverExtensions
    {
        public static IApplicationBuilder UseCommandReceiver( this IApplicationBuilder builder, string routePrefix, Action<ICommandReceiverOptions> configuration )
        {
            ICommandRegistry map = new DefaultCommandRegistry();
            ICommandReceiverOptions options = new DefaultReceiverOptions(routePrefix, map );
            configuration( options );

            return builder.UseMiddleware<CommandReceiver>( map );
        }

        public static void AddCommandReceiver( this IServiceCollection services )
        {
            services.AddSingleton<ICommandReceiver, DefaultCommandReceiver>();
            services.AddSingleton<ICommandRequestFactory, DefaultCommandFactory>();
            services.AddSingleton<ICommandResponseSerializer, DefaultCommandResponseSerializer>();
            services.AddSingleton<ICommandRunnerHostSelector, DefaultCommandRunnerHostSelector>();
            services.AddSingleton<ICommandHandlerFactory, DefaultCommandHandlerFactory>();
            services.AddSingleton<ICommandFileWriter, DefaultCommandFileStore>();
            services.AddSingleton<ICommandResponseDispatcher>( ( sp ) =>
           {
               IConnectionManager connectionManager = sp.GetService<IConnectionManager>();
               if( connectionManager != null ) return new HubResponseDispatcher( connectionManager );

               return new NullResponseDispatcher();
           } );
            services.AddSignalR( o =>
            {
                o.Hubs.EnableDetailedErrors = true;
            } );
        }
    }
}
