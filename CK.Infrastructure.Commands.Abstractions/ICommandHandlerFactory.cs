using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Infrastructure.Commands.Framework;

namespace CK.Infrastructure.Commands
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler Create( Type handlerType );
    }
}
