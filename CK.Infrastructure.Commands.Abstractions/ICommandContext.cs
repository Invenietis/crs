using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Infrastructure.Commands.Framework;

namespace CK.Infrastructure.Commands
{
    public interface ICommandContext
    {
        ICommandRequest Request { get; }

        ICommandResponse Response { get; }

        Type HandlerType { get; }

        bool IsAsynchronous { get; }

        IReadOnlyCollection<BlobRef> Blobs { get; }

        IEventDispatcher EventDispatcher { get; }

        ICommandHandlerFactory HandlerFactory { get; }
    }

}
