using Microsoft.Extensions.DependencyInjection;
using System;

namespace CK.Crs
{
    public interface ICrsCoreBuilder
    {
        IServiceCollection Services { get; }
        ICommandRegistry Registry { get; }
        /// <summary>
        /// Holds 
        /// </summary>
        ICrsModel Model { get; }

        /// <summary>
        /// Adds a custom <see cref="ICommandReceiver"/> to the crs core builder.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        void AddReceiver<T>() where T : class, ICommandReceiver;

        /// <summary>
        /// Adds a custom <see cref="IResultDispatcher"/> to the crs core builder.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="protocol"></param>
        void AddDispatcher<T>( string protocol ) where T : class, IResultDispatcher;
    }
}
