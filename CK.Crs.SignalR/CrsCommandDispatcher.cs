using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace CK.Crs.SignalR
{
    public class CrsCommandDispatcher : ICommandResponseDispatcher
    {
        readonly Lazy<IHubContext<ICrsClientProxy>> _context;
        readonly ICommandRunningStore _store;

        public CrsCommandDispatcher( ICommandRunningStore store )
        {
            _store = store;
            _context = new Lazy<IHubContext<ICrsClientProxy>>(
                () => GlobalHost.ConnectionManager.GetHubContext<ICrsClientProxy>( "crs" ) );
        }

        public virtual async Task DispatchAsync( string callbackId, CommandResponse response, CancellationToken cancellationToken = default( CancellationToken ) )
        {
            await _store.RemoveCommandAsync( callbackId, response.CommandId );

            await _context.Value.Clients.Client( callbackId ).OnCommandComplete( response );
        }
    }
}
