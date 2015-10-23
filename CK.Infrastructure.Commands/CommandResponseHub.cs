using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace CK.Infrastructure.Commands
{
    [HubName("commandResponse")]
    public class CommandResponseHub : Hub
    {
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
