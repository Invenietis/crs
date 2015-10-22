using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandContext
    {
        ICommandRequest Request { get; }

        ICommandResponse Response { get; }

        Type HandlerType { get; }

        IReadOnlyCollection<BlobRef> Blobs { get; }

        ICommandResponseDispatcher EventDispatcher { get; }

        ICommandHandlerFactory HandlerFactory { get; }
    }

}
