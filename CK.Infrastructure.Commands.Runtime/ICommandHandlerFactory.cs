using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// Defines the contract for a factory of <see cref="ICommandHandler"/>.
    /// </summary>
    public interface ICommandHandlerFactory
    {
        /// <summary>
        /// Creates a <see cref="ICommandHandler"/> for the given handler <see <paramref name="handlerType"/>.
        /// </summary>
        /// <param name="handlerType">The <see cref="Type"/> of the handler</param>
        /// <returns>An instance of <see cref="ICommandHandler"/> or null.</returns>
        ICommandHandler Create( Type handlerType );
    }
}
