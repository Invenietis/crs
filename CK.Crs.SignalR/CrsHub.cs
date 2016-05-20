using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace CK.Crs.SignalR
{
    [HubName( "crs" )]
    public class CrsHub : Hub
    {
        readonly ICommandRunningStore _store;

        public CrsHub( ICommandRunningStore store )
        {
            _store = store;
        }

        public Task<IReadOnlyCollection<Guid>> GetRunningCommands()
        {
            return _store.GetRunningCommands( Context.ConnectionId );
        }

        public Task SetRunningCommand( Guid commandId )
        {
            return _store.AddCommandAsync( Context.ConnectionId, commandId );
        }

        public Task RemoveRunningCommand( Guid commandId )
        {
            return _store.RemoveCommandAsync( Context.ConnectionId, commandId );
        }

        public override Task OnConnected()
        {
            return base.OnConnected();
        }

        public override Task OnDisconnected( bool stopCalled )
        {
            return base.OnDisconnected( stopCalled );
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }
    }
}
