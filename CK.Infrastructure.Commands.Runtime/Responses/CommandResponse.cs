using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    internal abstract class CommandResponse : ICommandResponse
    {
        internal CommandResponse( Guid commandId )
        {
            CommandId = commandId;
        }

        public CommandResponseType ResponseType { get; protected set; }

        public object Payload { get; protected set; }

        public Guid CommandId { get; private set; }
    }

}
