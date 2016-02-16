using System;
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
    public class CommandReceiverMiddleware
    {
        /// <summary>
        /// Shared options
        /// </summary>
        private readonly RequestDelegate _next;
        private readonly ICommandRouteCollection _routes;

        public CommandReceiverMiddleware( RequestDelegate next, ICommandRouteCollection routes )
        {
            _next = next;
            _routes = routes;
        }

        public async Task Invoke( HttpContext httpContext )
        {
            // __types -> 
            var monitor = new Core.ActivityMonitor(httpContext.Request.Path );

            var routedCommandDescriptor = _routes.FindCommandDescriptor( httpContext.Request.Path );
            if( routedCommandDescriptor == null )
            {
                if( _next != null ) await _next( httpContext );
            }
            else
            {
                ICommandBinder binder = ResolveCommandBinder( httpContext );
                ICommandRequest commandRequest = await binder.BindCommand( routedCommandDescriptor, httpContext.Request, httpContext.RequestAborted );
                if( commandRequest == null )
                {
                    string msg = String.Format(
                        "A valid command definition has been infered from routes, but the command type {0} failed to be instanciated.",
                        routedCommandDescriptor.Descriptor.CommandType.Name );

                    throw new InvalidOperationException( msg );
                }

                ICommandReceiver commandReceiver = ResolveCommandReceiver( httpContext );
                ICommandResponse commandResponse = await commandReceiver.ProcessCommandAsync( commandRequest, monitor, httpContext.RequestAborted );
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
                throw new InvalidOperationException( "An implementation of ICommandBinder must be registered into the ApplicationServices" );
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
}
