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
        CommandDescriptor Find( string commandReceiverPath, string commandName );

        ICommandRegistry Register( CommandDescriptor commandRegistration );
    }
}
