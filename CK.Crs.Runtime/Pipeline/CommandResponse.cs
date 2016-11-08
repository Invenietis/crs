using System;
using System.Collections.Generic;

namespace CK.Crs.Runtime
{
    public abstract class CommandResponse
    {
        protected CommandResponse( CommandResponseType responseType, Guid commandId )
        {
            ResponseType = (char)responseType;
            CommandId = commandId;
            Headers = new Dictionary<string, string>();
        }

        public Guid CommandId { get; private set; }

        public char ResponseType { get; protected set; }

        public object Payload { get; protected set; }

        public IDictionary<string, string> Headers { get; }
    }
}
