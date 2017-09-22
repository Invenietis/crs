using CK.Crs;
using CK.Crs.SignalR;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsSignalRDependencyInjectionExtensions
    {
        public static ICrsCoreBuilder AddSignalR( this ICrsCoreBuilder builder )
        {
            // The CommandJobQueue is added to the services: it will be disposed when the underlying container is disposed.
            builder.Services.AddSignalR();
            builder.Services.AddSingleton<IClientDispatcher, SignalRDispatcher>();
            return builder;
        }
    }
}
