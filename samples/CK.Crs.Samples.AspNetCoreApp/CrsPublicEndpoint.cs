using CK.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace CK.Crs.Samples.AspNetCoreApp
{

    [Route( "crs-dispatcher" )]
    public class SimpleCrsEndpoint<T> : ICrsReceiver<T> where T : class
    {
        ICommandDispatcher _dispatcher;
        public SimpleCrsEndpoint( ICommandDispatcher dispatcher )
        {
            _dispatcher = dispatcher;
        }

        [HttpPost( "[Action]" ), NoAmbientValuesValidation]
        public async Task<Response> ReceiveCommand( T command, ICommandContext context )
        {
            await _dispatcher.PostAsync( command, context );

            return new Response( ResponseType.Asynchronous, context.CommandId );
        }
    }

    [Route( "crs-sender" )]
    public class CrsPublicEndpoint<T> : DefaultCrsReceiver<T> where T : class
    {
        public CrsPublicEndpoint( ICommandSender sender, IClientEventStore eventStore ) : base( sender, eventStore ) { }

        [HttpPost( "[Action]" ), NoAmbientValuesValidation]
        public override Task<Response> ReceiveCommand( T command, ICommandContext context ) => base.ReceiveCommand( command, context );

        [HttpGet]
        public override Task<IEnumerable<IEventFilter>> Listeners( string callerId ) => base.Listeners( callerId );

        [HttpPut( "{eventName}" )]
        public override Task AddListener( string eventName, IListenerContext context ) => base.AddListener( eventName, context );

        [HttpDelete( "{eventName}" )]
        public override Task RemoveListener( string eventName, IListenerContext context ) => base.RemoveListener( eventName, context );
    }

    public class CreateUserCommand
    {
        public int ActorId { get; set; }
        public int AuthenticatedActorId { get; set; }
        public string UserName { get; set; }

        public class Result
        {
            public int UserId { get; set; }
        }
    }
}