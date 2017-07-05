using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Core
{
    /// <summary>
    /// <see cref="IAmbientValueProvider"/> factory which should creates a new instance each time create is called.
    /// </summary>
    public interface IAmbientValueProviderFactory
    {
        /// <summary>
        /// Creates an instance of <see cref="IAmbientValueProvider"/> from a Type.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns>An instance or null if type cannot be created.</returns>
        IAmbientValueProvider Create( IAmbientValueProviderDescriptor descriptor );
    }
}
