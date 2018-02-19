namespace CK.Crs
{
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
        /// <param name="commandId"></param>
        public Response( string commandId ) : base( Crs.ResponseType.Synchronous, commandId )
        {
        }
        /// <summary>
        /// 
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="commandId"></param>
        public Response( string commandId, T payload ) : this( Crs.ResponseType.Synchronous, commandId, payload )
        {
        }

        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="commandId"></param>
        public Response( char responseType, string commandId, T payload ) : base( responseType, commandId )
        {
            Payload = payload;
        }

        /// <summary>
        /// Creates a CommandResponse
        /// </summary>
        /// <param name="responseType">The type of a response.</param>
        /// <param name="commandId"></param>
        protected Response( ResponseType responseType, string commandId, T payload ) : base( responseType, commandId )
        {
            Payload = payload;
        }

        /// <summary>
        /// The raw response of the command. Can be null.
        /// </summary>
        public T Payload { get; set; }
    }
}
