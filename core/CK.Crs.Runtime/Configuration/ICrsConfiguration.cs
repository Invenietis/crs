using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Configuration
{
    public interface ICrsConfiguration
    {
        ICrsConfiguration Commands( Action<ICommandRegistry> commandsConfiguration );
    }
}
