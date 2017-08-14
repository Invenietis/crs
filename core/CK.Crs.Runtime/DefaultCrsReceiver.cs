﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Core;
using System.Linq;
using System;

namespace CK.Crs
{
    public abstract class DefaultCrsReceiver<T> : ICrsReceiver<T>, ICrsListener where T : class
    {
        readonly ICommandSender _sender;
        readonly IClientEventStore _liveEventStore;

        public DefaultCrsReceiver( ICommandSender sender, IClientEventStore liveEventStore )
        {
            _sender = sender;
            _liveEventStore = liveEventStore;
        }

        public virtual async Task<Response> ReceiveCommand( T command, ICommandContext context )
        {
            object result = null;
            Response response = null;

            using( context.Monitor.CollectEntries( ( errors ) =>
            {
                if( result == null )
                {
                    response = new ErrorResponse( string.Join( Environment.NewLine, errors.Select( e => e.ToString() ) ), context.CommandId );
                }
            } ) )
            {
                result = await _sender.SendAsync( command, context );
                response = new Response( ResponseType.Synchronous, context.CommandId )
                {
                    Payload = result
                };
            }

            return response;
        }

        public virtual Task AddListener( string eventName, IListenerContext context )
        {
            return _liveEventStore.AddEventFilter( eventName, context.ClientId );
        }

        public virtual Task RemoveListener( string eventName, IListenerContext context )
        {
            return _liveEventStore.RemoveEventFilter( eventName, context.ClientId );
        }

        public virtual Task<IEnumerable<IEventFilter>> Listeners( string clientId )
        {
            return _liveEventStore.GetEventFilters( clientId );
        }
    }
}
