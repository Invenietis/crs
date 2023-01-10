using System;
using System.Collections.Generic;

namespace CK.Crs
{
    /// <summary>
    /// Defines a response of a command.
    /// A response can be of VISAM type defined by the enum <see cref="ResponseType"/>.
    /// </summary>
    public abstract class Response
    {
        public Response()
        {
        }

        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="commandId"></param>
        protected Response( char responseType, string commandId )
        {
            ResponseType = responseType;
            CommandId = commandId;
        }

        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="commandId"></param>
        protected Response( ResponseType responseType, string commandId ) : this( (char)responseType, commandId )
        {
        }

        /// <summary>
        /// A unique id for the command
        /// </summary>
        public string CommandId { get; set; }

        /// <summary>
        /// The VISAM response type. See <see cref="Crs.ResponseType"/>
        /// </summary>
        public char ResponseType { get; set; }

    }

    
}
