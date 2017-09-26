using CK.Core;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.SignalR
{
    class ConnectionManager : ICrsConnectionManager
    {
        public List<CallerId> ActiveConnections { get; }

        public ConnectionManager()
        {
            ActiveConnections = new List<CallerId>();
        }

        public Task AddConnection( IActivityMonitor monitor, CallerId correlation )
        {
            ActiveConnections.Add( correlation ); // new CommandCorrelation( connectionId, principal.Identity.Name ) );
            return Task.CompletedTask;
        }

        public Task RemoveConnection( IActivityMonitor monitor, CallerId correlation )
        {
            ActiveConnections.Remove( correlation );// new CommandCorrelation( connectionId, principal.Identity.Name ) );
            return Task.CompletedTask;
        }

    }
}
