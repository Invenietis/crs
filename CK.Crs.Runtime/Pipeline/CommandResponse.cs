using System;

namespace CK.Crs.Runtime
{
    public abstract class CommandResponse
    {
        protected CommandResponse( Guid commandId )
        {
            CommandId = commandId;
        }

        public Guid CommandId { get; private set; }

        public CommandResponseType ResponseType { get; protected set; }

        public object Payload { get; protected set; }
    }
}
