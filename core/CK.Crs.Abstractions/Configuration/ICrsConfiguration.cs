using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public interface ICrsConfiguration
    {
        ICrsConfiguration AddCommands(Action<IRequestRegistry> registryConfiguration);

        ICrsConfiguration AddReceivers(Action<ICrsEndpointConfigurationRoot> endpointConfiguration);
    }
}
