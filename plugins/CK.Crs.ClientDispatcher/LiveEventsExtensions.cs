using CK.Communication.WebSockets;
using CK.Crs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LiveEventsExtensions
    {
        public static ICrsCoreBuilder AddClientDispatcher( this ICrsCoreBuilder builder )
        {
            return AddClientDispatcher( builder, new ClientDispatcherOptions() );
        }

        public static ICrsCoreBuilder AddClientDispatcher( this ICrsCoreBuilder builder, ClientDispatcherOptions options )
        {
            builder.Services.AddWebSockets();
            builder.Services.AddSingleton<IClientEventStore, InMemoryLiveEventStore>();
            builder.Services.AddSingleton<IClientDispatcher, WebSocketWebClientDispatcher>();
            builder.Services.AddSingleton( options );

            return builder;
        }
    }
}
