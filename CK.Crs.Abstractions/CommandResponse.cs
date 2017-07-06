using System;
using System.Collections.Generic;

namespace CK.Crs
{
    /// <summary>
    /// Defines a response of a command.
    /// A response can be of VISAM type defined by the enum <see cref="CommandResponseType"/>.
    /// </summary>
    public abstract class CommandResponse<T>
    {
        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="commandId"></param>
        protected CommandResponse( CommandResponseType responseType, Guid commandId )
        {
            ResponseType = (char)responseType;
            CommandId = commandId;
        }

        /// <summary>
        /// A unique id for the command
        /// </summary>
        public Guid CommandId { get; private set; }

        /// <summary>
        /// The VISAM response type. See <see cref="CommandResponseType"/>
        /// </summary>
        public char ResponseType { get; protected set; }

        /// <summary>
        /// The raw response of the command. Can be null.
        /// </summary>
        public T Payload { get; protected set; }
    }

    /// <summary>
    /// Defines a response of a command.
    /// A response can be of VISAM type defined by the enum <see cref="CommandResponseType"/>.
    /// </summary>
    public abstract class CommandResponse : CommandResponse<object>
    {
        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="commandId"></param>
        protected CommandResponse( CommandResponseType responseType, Guid commandId ) : base( responseType, commandId )
        {
        }
    }
}
