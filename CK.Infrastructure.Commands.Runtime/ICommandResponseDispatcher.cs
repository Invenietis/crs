using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// </summary>
    public interface ICommandResponseDispatcher
    {
        /// <summary>
        /// Dispatch a <see cref="ICommandResponse"/> to the identified callback identifier.
        /// </summary>
        /// <param name="callbackId"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task DispatchAsync( string callbackId, ICommandResponse response );
    }

}
