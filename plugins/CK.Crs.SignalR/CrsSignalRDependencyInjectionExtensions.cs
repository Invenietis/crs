using System;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs;
using CK.Crs.SignalR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class CrsSignalRDependencyInjectionExtensions
    {
        public static ICrsCoreBuilder AddSignalR( this ICrsCoreBuilder builder, Action<CrsSignalROptions> configure = null )
        {
            builder.Services.AddSignalR();
            builder.AddDispatcher<SignalRDispatcher>( CrsSignalRContextExtensions.CallerIdProtocol );
            builder.Services.AddSingleton<IStartupFilter, CrsSignalRStartupFilter>();

            if( configure == null ) configure = new Action<CrsSignalROptions>( _ => { } );
            builder.Services.Configure( configure );
            return builder;
        }
    }
}
