using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
    public interface ICommandDecoratorFactory
    {
        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="decoratorType">The type of the decorator to create</param>
        /// <returns>An instance of <see cref="ICommandDecorator"/> or null.</returns>
        ICommandDecorator CreateDecorator( Type decoratorType );
    }
}
