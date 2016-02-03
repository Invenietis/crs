using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public interface ICommandRegistry
    {
        CommandDescriptor Register( Type commandType, Type handlerType, string commandName, bool isLongRunning, params Type[] decorators );

        IEnumerable<CommandDescriptor> Registration { get; }
    }
}
