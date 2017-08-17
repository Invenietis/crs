using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public interface ICrsConfiguration
    {
        ICrsConfiguration Commands( Action<ICommandRegistry> commandsConfiguration );

        ICrsConfiguration Endpoints( Action<ICrsEndpointConfigurationRoot> endpointConfiguration );
    }
}
