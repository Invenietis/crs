using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public enum CommandResponseType
    {
        Error = -1,
        Direct = 0,
        Deferred = 1
    }

    public interface ICommandResponse
    {
        Guid CommandId { get; }

        CommandResponseType ResponseType { get; set; }

        object Payload { get; set; }
    }


}
