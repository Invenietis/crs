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
            var commandRegistration = FindCommandRoute( httpContext );
            if( commandRegistration == null )
            {
                if( _next != null )
                {
                    await _next( httpContext );
                    return;
                }
            }

            //1. Concretize Stream into valid .NET DTO Commands

            ICommand command = ResolveCommandFactory( httpContext ).CreateCommand( commandRegistration, httpContext.Request.Body );
            if( command == null )
            {
                string msg = String.Format( "A valid command definition has been infered from routes, but the command type {0} failed to be instanciated.", commandRegistration.CommandType.Name );
                throw new InvalidOperationException( msg );
            }
            //2. Sends to the bus
            ICommandResult result = await ResolveCommandBus( httpContext ).SendAsync( command );
            if( result == null )
            {
                string msg = "The command bus must returns a valid ICommandResult, even in failure cases.";
                throw new InvalidOperationException( msg );
            }
            await WriteCommandResponse( httpContext, result );
            // Do not call the next handler as the request has been handled by this middleware.
        }

        protected virtual async Task WriteCommandResponse( HttpContext httpContext, ICommandResult result )
        {
            httpContext.Response.ContentType = httpContext.Request.ContentType;
            await SelectCommandSerializer( httpContext ).SerializeCommandResult( result, httpContext.Response.Body );
        }

        protected virtual CommandRouteRegistration FindCommandRoute( HttpContext context )
        {
            //context.Request.Path.StartsWithSegments( _requestUrlPath )

            return ResolveRouteMap( context ).FindCommandRoute( context.Request.Path, _options );
        }

        protected virtual ICommandRouteMap ResolveRouteMap( HttpContext context )
        {
            ICommandRouteMap map = context.ApplicationServices.GetService<ICommandRouteMap>();
            if( map == null )
            {
                throw new InvalidOperationException( "An implementation of ICommandRouteMap must be registered into the ApplicationServices" );
            }
            return map;
        }

        protected virtual ICommandFactory ResolveCommandFactory( HttpContext context )
        {
            ICommandFactory commandFactory = context.ApplicationServices.GetService<ICommandFactory>();
            if( commandFactory == null )
            {
                throw new InvalidOperationException( "An implementation of ICommandFactory must be registered into the ApplicationServices" );
            }
            return commandFactory;
        }

        protected virtual ICommandBus ResolveCommandBus( HttpContext context )
        {
            ICommandBus commandBus = context.ApplicationServices.GetService<ICommandBus>();
            if( commandBus == null )
            {
                throw new InvalidOperationException( "An implementation of ICommandBus must be registered into the ApplicationServices" );
            }
            return commandBus;
        }
        protected virtual ICommandResultSerializer SelectCommandSerializer( HttpContext context )
        {
            ICommandResultSerializer commandSerializer = context.ApplicationServices.GetService<ICommandResultSerializer>();
            if( commandSerializer == null )
            {
                throw new InvalidOperationException( "An implementation of ICommandSerializer must be registered into the ApplicationServices" );
            }
            return commandSerializer;
        }
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
            services.AddSingleton<ICommandFactory, DefaultCommandFactory>();
            services.AddSingleton<ICommandResultSerializer, DefaultCommandResultSerializer>();
            services.AddSingleton<ICommandBus, DefaultCommandBus>();
            services.AddSingleton<ICommandRouteMap, DefaultCommandRouteMap>();
            services.AddSingleton<ICommandTypeSelector, ConventionCommandTypeSelector>();
        }
    }
}
