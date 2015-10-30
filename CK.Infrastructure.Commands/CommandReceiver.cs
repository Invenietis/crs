﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;

namespace CK.Infrastructure.Commands
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    public class CommandReceiver
    {
        private readonly CommandReceiverOptions _options;
        private readonly RequestDelegate _next;

        private readonly PathString _routePrefix;
        public CommandReceiver( RequestDelegate next, IOptions<CommandReceiverOptions> options, PathString routePrefix )
        {
            _next = next;
            _options = options.Value;
            _routePrefix = routePrefix;
        }

        public async Task Invoke( HttpContext httpContext )
        {
            var commandDescription = _options.Registry.Find( _routePrefix, httpContext.Request.Path );
            if( commandDescription == null )
            {
                if( _next != null ) await _next( httpContext );
            }
            else
            {
                ICommandBinder binder = ResolveCommandBinder( httpContext );
                ICommandRequest commandRequest = await binder.BindCommand( commandDescription, httpContext.Request, httpContext.RequestAborted );
                if( commandRequest == null )
                {
                    string msg = String.Format( "A valid command definition has been infered from routes, but the command type {0} failed to be instanciated.", commandDescription.CommandType.Name );
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
        }

        #region Resolvers

        protected virtual ICommandBinder ResolveCommandBinder( HttpContext context )
        {
            ICommandBinder commandFactory = context.ApplicationServices.GetService<ICommandBinder>();
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
        public static IApplicationBuilder UseCommandReceiver( this IApplicationBuilder builder, PathString routePrefix )
        {
            if( string.IsNullOrWhiteSpace( routePrefix ) ) throw new ArgumentNullException( nameof( routePrefix ) );

            return builder.Map( routePrefix, ( app ) =>
            {
                app.UseMiddleware<CommandReceiver>( routePrefix );
            } );
        }
        public static void AddCommandReceiver( this IServiceCollection services, bool enableLongRunningSupport = false )
            => AddCommandReceiver( services, ( o ) => o.EnableLongRunningSupport = enableLongRunningSupport );

        public static void AddCommandReceiver( this IServiceCollection services, Action<CommandReceiverOptions> options )
        {
            services.AddOptions();
            services.AddSingleton<ICommandReceiver, DefaultCommandReceiver>();
            services.AddSingleton<ICommandValidator, DefaultCommandValidator>();
            services.AddSingleton<ICommandBinder, DefaultCommandBinder>();
            services.AddSingleton<ICommandResponseSerializer, DefaultCommandResponseSerializer>();
            services.AddSingleton<ICommandRunnerHostSelector, CommandRunnerHostSelector>();
            services.AddSingleton<ICommandHandlerFactory, CommandHandlerFactory>();
            services.AddSingleton<ICommandFileWriter, DefaultCommandFileStore>();
            services.AddSingleton<ICommandResponseDispatcher>( ( sp ) =>
            {
                IConnectionManager connectionManager = sp.GetService<IConnectionManager>();
                if( connectionManager != null ) return new HubResponseDispatcher( connectionManager );

                return new NullResponseDispatcher();
            } );

            services.Configure( options );
        }
    }
}
