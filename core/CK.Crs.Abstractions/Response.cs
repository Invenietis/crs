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
        /// <param name="requestId"></param>
        protected Response( char responseType, Guid requestId )
        {
            ResponseType = responseType;
            RequestId = requestId;
        }

        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="requestId"></param>
        protected Response( ResponseType responseType, Guid requestId ) : this( (char)responseType, requestId )
        {
        }

        /// <summary>
        /// A unique id for the command
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// The VISAM response type. See <see cref="Crs.ResponseType"/>
        /// </summary>
        public char ResponseType { get; set; }

    }

    /// <summary>
    /// Defines a response of a command.
    /// A response can be of VISAM type defined by the enum <see cref="Crs.ResponseType"/>.
    /// </summary>
    public class Response<T> : Response
    {
        public Response()
        {
        }

        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="requestId"></param>
        public Response( Guid requestId ) : base( CK.Crs.ResponseType.Synchronous, requestId )
        {
        }

        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="requestId"></param>
        public Response( char responseType, Guid requestId ) : base( responseType, requestId )
        {
        }

        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="requestId"></param>
        protected Response( ResponseType responseType, Guid requestId ) : base( responseType, requestId )
        {
        }

        /// <summary>
        /// The raw response of the command. Can be null.
        /// </summary>
        public T Payload { get; set; }
    }
}
