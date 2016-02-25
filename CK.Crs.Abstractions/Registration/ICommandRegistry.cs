using System.Collections.Generic;

namespace CK.Crs
{
    public interface ICommandRegistry
    {
        void Register( CommandDescriptor descriptor );

        IEnumerable<CommandDescriptor> Registration { get; }

        bool EnableLongRunningCommands { get; set; }
    }
}
