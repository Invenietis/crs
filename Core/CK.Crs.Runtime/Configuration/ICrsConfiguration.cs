using System;

namespace CK.Crs.Configuration
{
    public interface ICrsConfiguration
    {
        ICrsConfiguration Commands( Action<ICommandRegistry> commandsConfiguration );
    }
}
