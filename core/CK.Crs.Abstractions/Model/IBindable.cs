using System;

namespace CK.Crs
{
    public interface IBindable
    {

        /// <summary>
        /// Gets the default <see cref="ICommandBinder"/> associates with this endpoint
        /// </summary>
        Type Binder { get; }
    }
}
