using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    /// <summary>
    /// A command response is returned by a <see cref="ICommandReceiver"/> when it finish processing the command
    /// The type of the response is defined by the <see cref="CommandResponseType"/>.
    /// The payload of the response can contain any information usefull for the client to finaly obtain the result of its command.
    /// </summary>
    public interface ICommandResponse
    {
        /// <summary>
        /// The unique identifier of the processing command. Can be considered as a correlation identifier.
        /// </summary>
        Guid CommandId { get; }

        /// <summary>
        /// Gets the type of the response.
        /// </summary>
        CommandResponseType ResponseType { get; }

        /// <summary>
        /// Gets the response data.
        /// </summary>
        object Payload { get;  }
    }


}
