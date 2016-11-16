using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{

    public interface IAmbientValuesRegistration
    {
        /// <summary>
        /// Gets all <see cref="IAmbientValueProviderDescriptor"/> registered so far.
        /// </summary>
        IEnumerable<IAmbientValueProviderDescriptor> AmbientValues { get; }

        /// <summary>
        /// Registers an ambient <see cref="IAmbientValueProvider"/> capable to resolve the value of an ambient parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the ambient parameter managed by this <see cref="IAmbientValueProvider"/></param>
        void Register<T>( string name ) where T : class, IAmbientValueProvider;

        /// <summary>
        /// Register a deferred pointer to an instance of <see cref="IAmbientValueProvider"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        void Register( string name, Func<IServiceProvider, IAmbientValueProvider> provider );

        /// <summary>
        /// Gets an <see cref="IAmbientValueProviderDescriptor"/> by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAmbientValueProviderDescriptor GetByName( string name );
    }
}