using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace CK.Infrastructure.Commands
{

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
