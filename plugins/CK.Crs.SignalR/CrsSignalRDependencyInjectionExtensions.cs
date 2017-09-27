using System;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs;
using CK.Crs.SignalR;
using Microsoft.AspNetCore.Http.Features;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsSignalRDependencyInjectionExtensions
    {
        public static ICrsCoreBuilder AddSignalR( this ICrsCoreBuilder builder )
        {
            builder.Services.AddSignalR();
            builder.AddDispatcher<SignalRDispatcher>( CrsSignalRContextExtensions.CallerIdProtocol );
            builder.Services.AddSingleton<SignalRDispatcher>();

            return builder;
        }
    }
}
