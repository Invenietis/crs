using CK.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Sockets.Client;
using Microsoft.AspNetCore.Sockets.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.SignalR
{
    public interface ICrsHub
    {
        void ReceiveResult( string message );
    }

    public interface IActivityMonitorFeature
    {
        IActivityMonitor Monitor { get; }
    }

    public class CrsHub : Hub<ICrsHub>
    {
        private readonly ICrsConnectionManager _connectionManager;

        public CrsHub( ICrsConnectionManager connectionManager )
        {
            _connectionManager = connectionManager;
        }

        public override Task OnConnectedAsync()
        {
            var mon = Context.Connection.Features.Get<IActivityMonitorFeature>().Monitor;
            var callerId = Context.GetCallerId();
            return _connectionManager.AddConnection( mon, callerId );
        }

        public override Task OnDisconnectedAsync( Exception exception )
        {
            var mon = Context.Connection.Features.Get<IActivityMonitorFeature>().Monitor;
            var callerId = Context.GetCallerId();
            return _connectionManager.RemoveConnection( mon, callerId );
        }
    }
}
