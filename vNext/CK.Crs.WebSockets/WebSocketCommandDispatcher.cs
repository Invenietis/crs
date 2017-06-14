using System;
using System.Threading;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Execution;

namespace CK.Crs.WebSockets
{
    public class WebSocketCommandDispatcher : ICommandResponseDispatcher
    {
        readonly ICommandRunningStore _store;

        public WebSocketCommandDispatcher( ICommandRunningStore store )
        {
            _store = store;
        }

        public virtual async Task DispatchAsync( string callbackId, CommandResponse response, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            await _store.RemoveCommandAsync( callbackId, response.CommandId );

            //await _context.Value.Clients.Client( callbackId ).OnCommandComplete( response );
        }
    }
}
