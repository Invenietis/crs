using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public interface IAmbientValuesRegistration
    {
        /// <summary>
        /// Registers an ambient <see cref="IAmbientValueProvider"/> capable to resolve the value of an ambient parameter.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the ambient parameter managed by this <see cref="IAmbientValueProvider"/></param>
        void Register<T>( string name ) where T : IAmbientValueProvider;

        void Register( string name, Func<IAmbientValueProvider> provider );
    }

    public interface IAmbientValues : IAmbientValuesRegistration
    {
        /// <summary>
        /// Gets wether an ambient parameter has been defined or not.
        /// </summary>
        /// <param name="name">The name of the ambient parameter to check.</param>
        /// <returns>True if the ambient parameter is defined. False otherwise.</returns>
        bool IsDefined( string name );

        /// <summary>
        /// Gets the value of an ambient parameter.
        /// </summary>
        /// <param name="name">The name of the value to obtain.</param>
        /// <returns>A task, when resolved, the value of the ambient parameter, or null if not found.</returns>
        Task<T> GetValueAsync<T>( string name );
    }

    public interface IAmbientValueProvider
    {
        /// <summary>
        /// Gets the value of an ambient parameter. 
        /// </summary>
        /// <param name="values">A reference to the all <see cref="IAmbientValues"/></param>
        /// <returns></returns>
        Task<object> GetValueAsync( IAmbientValues values );
    }

    public interface IAmbientValueProviderFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IAmbientValueProvider"/> from a Type.
        /// </summary>
        /// <param name="providerType"></param>
        /// <returns>An instance or null if type cannot be created.</returns>
        IAmbientValueProvider Create( Type providerType );
    }
}