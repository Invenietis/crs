using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{

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
