using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.DependencyInjection;

namespace CK.Infrastructure.Commands
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    public class CommandReceiver
    {
        private readonly RequestDelegate _next;
        private readonly ICommandReceiverOptions _options;

        public CommandReceiver( RequestDelegate next, ICommandReceiverOptions options )
        {
            _next = next;
            _options = options;
        }

        public async Task Invoke( HttpContext httpContext )
        {
            var commandRegistration = ResolveRouteMap( httpContext ).FindCommandRoute( httpContext.Request.Path, _options );
            if( commandRegistration == null && _next != null )
            {
                await _next( httpContext );
                return;
            }

            ICommandRequestFactory commandRequestFactory = ResolveCommandFactory( httpContext );
            ICommandRequest commandRequest = commandRequestFactory.CreateCommand( commandRegistration, httpContext.Request.Body );
            if( commandRequest == null )
            {
                string msg = String.Format( "A valid command definition has been infered from routes, but the command type {0} failed to be instanciated.", commandRegistration.CommandType.Name );
                throw new InvalidOperationException( msg );
            }

            ICommandReceiver commandReceiver = ResolveCommandReceiver( httpContext );
            ICommandResponse commandResponse = await commandReceiver.ProcessCommandAsync( commandRequest );
            if( commandResponse == null )
            {
                string msg = String.Format( "A valid command response must be received" );
                throw new InvalidOperationException( msg );
            }

            ICommandResponseSerializer responseSerializer = ResolveCommandResponseSerializer( httpContext );
            responseSerializer.Serialize( commandResponse, httpContext.Response.Body );
        }

        #region Resolvers

        protected virtual ICommandRouteMap ResolveRouteMap( HttpContext context )
        {
            ICommandRouteMap map = context.ApplicationServices.GetService<ICommandRouteMap>();
            if( map == null )
            {
                throw new InvalidOperationException( "An implementation of ICommandRouteMap must be registered into the ApplicationServices" );
            }
            return map;
        }

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
            ICommandReceiverOptions options = new DefaultReceiverOptions(routePrefix);
            configuration( options );

            return builder.UseMiddleware<CommandReceiver>( options );
        }

        public static void AddCommandReceiver( this IServiceCollection services )
        {
            services.AddSingleton<ICommandReceiver, DefaultCommandReceiver>();
            services.AddSingleton<ICommandRequestFactory, DefaultCommandFactory>();
            services.AddSingleton<ICommandResponseSerializer, DefaultCommandResponseSerializer>();
            services.AddSingleton<ICommandRouteMap, DefaultCommandRouteMap>();
            services.AddSingleton<ICommandHandlerFactory, DefaultCommandHandlerFactory>();
            services.AddSingleton<ICommandFileWriter, DefaultCommandFileStore>();
        }
    }
}
