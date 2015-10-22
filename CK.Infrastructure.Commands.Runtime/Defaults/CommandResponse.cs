using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class CommandResponse : ICommandResponse
    {
        public Guid CommandId { get; set; }

        public CommandResponseType ResponseType { get; set; }

        public object Payload { get; set; }
    }

}
