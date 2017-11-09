using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Core
{
    /// <summary>
    /// Resolves the value of an ambient parameter.
    /// </summary>
    /// <remarks>
    /// The returned value is always "context-bounded" since this is by defintion an ambient value.
    /// </remarks>
    public interface IAmbientValues
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
        Task<IComparable> GetValueAsync( string name );

        /// <summary>
        /// Gets all the descriptors for the registered ambient parameters.
        /// </summary>
        IEnumerable<IAmbientValueProviderDescriptor> Descriptors { get; }
    }
}
