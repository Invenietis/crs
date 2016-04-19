using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
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
}
