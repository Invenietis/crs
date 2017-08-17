using CK.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [Route( "crs-sender" )]
    public class CrsSenderEndpoint<T> : IHttpCommandReceiver<T>, ICrsListener where T : class
    {
        readonly ICommandReceiver _sender;
        readonly IClientEventStore _eventStore;

        public CrsSenderEndpoint( ICommandReceiver sender, IClientEventStore eventStore )
        {
            _sender = sender;
            _eventStore = eventStore;
        }

        [HttpPost( "[Action]" ), NoAmbientValuesValidation]
        public async Task<Response> ReceiveCommand( T command, ICommandContext context, HttpContext httpContext )
        {
            object result = null;
            Response response = null;


                result = await _sender.ReceiveCommand( command, context );
                response = new Response( ResponseType.Synchronous, context.CommandId )
                {
                    Payload = result
                };

            return response;
        }

        [HttpGet]
        public virtual Task<IEnumerable<IEventFilter>> Listeners( string clientId )
        {
            return _eventStore.GetEventFilters( clientId );
        }

        [HttpPut( "{eventName}" )]
        public Task AddListener( string eventName, IListenerContext context )
        {
            return _eventStore.AddEventFilter( eventName, context.ClientId );
        }

        [HttpDelete( "{eventName}" )]
        public virtual Task RemoveListener( string eventName, IListenerContext context )
        {
            return _eventStore.RemoveEventFilter( eventName, context.ClientId );
        }
    }
}
