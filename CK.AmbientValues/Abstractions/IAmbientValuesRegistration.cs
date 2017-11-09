using System;
using System.Collections.Generic;

namespace CK.Core
{
    /// <summary>
    /// Ambient values registrations
    /// </summary>
    public interface IAmbientValuesRegistration
    {
        /// <summary>
        /// Gets all <see cref="IAmbientValueProviderDescriptor"/> registered so far.
        /// </summary>
        IEnumerable<IAmbientValueProviderDescriptor> AmbientValues { get; }

        /// <summary>
        /// Registers an ambient <see cref="IAmbientValueProvider"/> which should be able to resolve the value of an ambient parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the ambient parameter managed by this <see cref="IAmbientValueProvider"/></param>
        IAmbientValuesRegistration AddProvider<T>( string name, Action<IConfigurableAmbientValueProviderDescriptor> configure = null ) where T : class, IAmbientValueProvider;


        /// <summary>
        /// Register a deferred pointer to an instance of <see cref="IAmbientValueProvider"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        IAmbientValuesRegistration AddProvider( string name, Func<IServiceProvider, IAmbientValueProvider> provider, Action<IConfigurableAmbientValueProviderDescriptor> configure = null );

        /// <summary>
        /// Gets an <see cref="IAmbientValueProviderDescriptor"/> by its name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IAmbientValueProviderDescriptor GetByName( string name );
    }
}
