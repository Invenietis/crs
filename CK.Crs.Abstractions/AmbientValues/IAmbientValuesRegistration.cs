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

        /// <summary>
        /// Register a deferred pointer to an instance of <see cref="IAmbientValueProvider"/>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        void Register( string name, Func<IAmbientValueProvider> provider );
    }
}