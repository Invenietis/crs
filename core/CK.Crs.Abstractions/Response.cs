using System;
using System.Collections.Generic;

namespace CK.Crs
{
    /// <summary>
    /// Defines a response of a command.
    /// A response can be of VISAM type defined by the enum <see cref="Crs.ResponseType"/>.
    /// </summary>
    public abstract class Response<T>
    {
        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="requestId"></param>
        protected Response( ResponseType responseType, Guid requestId)
        {
            ResponseType = (char)responseType;
            RequestId = requestId;
        }

        /// <summary>
        /// A unique id for the command
        /// </summary>
        public Guid RequestId { get; private set; }

        /// <summary>
        /// The VISAM response type. See <see cref="Crs.ResponseType"/>
        /// </summary>
        public char ResponseType { get; protected set; }

        /// <summary>
        /// The raw response of the command. Can be null.
        /// </summary>
        public T Payload { get; set; }
    }

    /// <summary>
    /// Defines a response of a command.
    /// A response can be of VISAM type defined by the enum <see cref="ResponseType"/>.
    /// </summary>
    public class Response : Response<object>
    {
        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="requestId"></param>
        public Response( ResponseType responseType, Guid requestId ) : base( responseType, requestId )
        {
        }
    }
}
