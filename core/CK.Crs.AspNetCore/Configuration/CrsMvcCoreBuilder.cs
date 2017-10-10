using CK.Crs;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public sealed class CrsMvcCoreBuilder : ICrsCoreBuilder
    {
        public ICrsCoreBuilder CrsBuilder { get; internal set; }

        public IMvcCoreBuilder MvcBuilder { get; internal set; }

        public IServiceCollection Services => CrsBuilder.Services;

        public ICommandRegistry Registry => CrsBuilder.Registry;

        public ICrsModel Model => CrsBuilder.Model;

        public void AddReceiver<T>( Func<IServiceProvider, T> factoryFunction ) where T : class, ICommandReceiver
            => CrsBuilder.AddReceiver( factoryFunction );

        public void AddDispatcher<T>( string protocol ) where T : class, IResultDispatcher
            => CrsBuilder.AddDispatcher<T>( protocol );
    }
}
