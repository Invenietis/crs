using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandRegistry
    {
        void Register( CommandDescriptor descriptor );

        IEnumerable<CommandDescriptor> Registration { get; }
        bool EnableLongRunningCommands { get; set; }
    }
}
