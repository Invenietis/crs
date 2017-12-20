using Microsoft.Extensions.DependencyInjection;
using System;

namespace CK.Crs
{
    public interface ICrsCoreBuilder
    {
        IServiceCollection Services { get; }
        ICommandRegistry Registry { get; }
        ICrsModel Model { get; }
        void AddReceiver<T>( Func<IServiceProvider, T> factoryFunction = null ) where T : class, ICommandReceiver;
        void AddDispatcher<T>( string protocol ) where T : class, IResultDispatcher;
    }
}
