﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;
using CK.Crs.Runtime;

namespace CK.Crs
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    public class CommandReceiverOwinMiddleware : OwinMiddleware
    {
        /// <summary>
        /// Shared options
        /// </summary>
        private readonly ICrsHandler _handler;

        public CommandReceiverOwinMiddleware( ICrsHandler handler ) : this( null, handler )
        {
        }

        public CommandReceiverOwinMiddleware( OwinMiddleware next, ICrsHandler handler )
            : base( next )
        {
            if( handler == null ) throw new ArgumentNullException( nameof( handler ) );

            _handler = handler;
        }

        public Task InvokeAsync( IDictionary<string, object> environment )
        {
            return Invoke( new OwinContext( environment ) );
        }

        public async override Task Invoke( IOwinContext context )
        {
            var connectionId = context.Request.Query["c"];
            var commandRequest = new CommandRequest( context.Request.Path.Value, context.Request.Body, context.Authentication.User, connectionId );
            var hasResponse = await _handler.ProcessCommandAsync( commandRequest, context.Response.Body );
            if( hasResponse == false && Next != null )
                await Next.Invoke( context );
        }

    }
}