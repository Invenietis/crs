using System;

namespace CK.Crs.Runtime.Filtering
{
    public interface ICommandFilterFactory
    {
        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="filterType"></param>
        /// <returns></returns>
        ICommandFilter CreateFilter( Type filterType );
    }
}
