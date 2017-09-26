using CK.Communication.WebSockets;
using CK.Crs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ClientDispatcherConfigurationExtensions
    {
        public static ICrsCoreBuilder AddClientDispatcher( this ICrsCoreBuilder builder )
        {
            return AddClientDispatcher( builder, new ClientDispatcherOptions() );
        }

        public static ICrsCoreBuilder AddClientDispatcher( this ICrsCoreBuilder builder, ClientDispatcherOptions options )
        {
            builder.Services.AddWebSockets();
            builder.Services.AddSingleton<IClientEventStore, InMemoryLiveEventStore>();
            builder.Services.AddSingleton<IResultDispatcher, WebSocketWebClientDispatcher>();
            builder.Services.AddOptions();
            builder.Services.AddSingleton( Microsoft.Extensions.Options.Options.Create( options ) );

            return builder;
        }
    }
}
