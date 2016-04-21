using System;

namespace CK.Crs.Runtime
{
    public abstract class CommandResponse
    {
        protected CommandResponse( CommandResponseType responseType, Guid commandId )
        {
            ResponseType = (char)responseType;
            CommandId = commandId;
        }

        public Guid CommandId { get; private set; }

        public char ResponseType { get; protected set; }

        public object Payload { get; protected set; }
    }
}
