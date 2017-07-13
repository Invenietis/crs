using System.Collections.Generic;
using System.Threading.Tasks;

namespace CK.Crs
{
    public abstract class DefaultCrsEndpoint<T> : ICrsReceiver<T>, ICrsListener where T : class
    {
        readonly ICommandSender _sender;
        readonly ILiveEventStore _liveEventStore;

        public DefaultCrsEndpoint( ICommandSender sender, ILiveEventStore liveEventStore )
        {
            _sender = sender;
            _liveEventStore = liveEventStore;
        }

        public virtual async Task<Response> ReceiveCommand( T command, ICommandContext context )
        {
            var result = await _sender.SendAsync( command, context );

            return new Response( ResponseType.Synchronous, context.CommandId )
            {
                Payload = result
            };
        }

        public virtual Task AddListener( string eventName, IListenerContext context )
        {
            return _liveEventStore.RegisterListener( eventName, context.ClientId );
        }

        public virtual Task RemoveListener( string eventName, IListenerContext context )
        {
            return _liveEventStore.RemoveListener( eventName, context.ClientId );
        }

        public virtual Task<IEnumerable<ILiveEventModel>> Listeners( string callerId )
        {
            return _liveEventStore.GetListeners( callerId );
        }
    }
}
