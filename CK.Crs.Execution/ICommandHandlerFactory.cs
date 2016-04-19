using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Execution
{
    public interface ICommandHandlerFactory
    {
        /// <summary>
        /// Creates an instance of the given type or return null if the type cannot be instanciated.
        /// </summary>
        /// <param name="handlerType">The type of the handler to create.</param>
        /// <returns></returns>
        ICommandHandler CreateHandler( Type handlerType );

        /// <summary>
        /// Releases a <see cref="ICommandHandler"/>.
        /// </summary>
        /// <param name="handler">The command handler to release.</param>
        void ReleaseHandler( ICommandHandler handler );
    }
}
