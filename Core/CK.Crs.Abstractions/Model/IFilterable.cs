using System;
using System.Collections.Generic;

namespace CK.Crs
{
    public interface IFilterable
    {
        /// <summary>
        /// Gets all filered configured for this command.
        /// </summary>
        IEnumerable<Type> Filters { get; }
    }
}
