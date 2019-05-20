using CK.Core;
using CK.Crs;
using CK.Crs.Hosting;
using CK.Crs.InMemory;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsInMemoryDependencyInjectionExtensions
    {
        public static ICrsCoreBuilder AddInMemoryReceiver( this ICrsCoreBuilder builder, Action<ICommandHost> hostConfiguration = null )
        {
            // The CommandJobQueue is added to the services: it will be disposed when the underlying container is disposed.

            builder.Services.AddSingleton<CommandJobQueue>();
            builder.AddReceiver<CommandReceiver>();

            builder.Services.AddSingleton<ICommandHost>( ( sp ) =>
            {
                var bJobService = new BackgroundJobService();
                hostConfiguration?.Invoke( bJobService );
                return bJobService;
            } );


            return builder;
        }
    }
}
