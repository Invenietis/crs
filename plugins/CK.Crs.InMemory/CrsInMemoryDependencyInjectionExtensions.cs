using CK.Crs;
using CK.Crs.InMemory;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsInMemoryDependencyInjectionExtensions
    {
        public static ICrsCoreBuilder AddInMemoryReceiver( this ICrsCoreBuilder builder )
        {
            // The CommandJobQueue is added to the services: it will be disposed when the underlying container is disposed.
            builder.Services.AddSingleton<CommandJobQueue>();
            builder.AddReceiver<CommandReceiver>();
            return builder;
        }
    }
}
