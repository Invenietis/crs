using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands.Abstractions
{
    public interface ICommandHandlerRegistry
    {
        ICommandHandlerRegistry Register( Type commandType, Type handlerType );
    }
}
