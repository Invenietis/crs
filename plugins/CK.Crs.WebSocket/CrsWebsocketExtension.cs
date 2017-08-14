using CK.Communication.WebSockets;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public static class CrsWebsocketExtension
    {
        public static ICrsCoreBuilder AddWebSockets( this ICrsCoreBuilder builder )
        {
            builder.Services.AddWebSockets();
            builder.Services.AddSingleton<IClientDispatcher, WebSocketWebClientDispatcher>();

            return builder;
        }
    }
}
