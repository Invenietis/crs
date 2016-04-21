using System.Collections.Generic;

namespace CK.Crs
{
    public interface ICommandRegistry
    {
        void Register( CommandDescription descriptor );

        IEnumerable<CommandDescription> Registration { get; }
    }
}
