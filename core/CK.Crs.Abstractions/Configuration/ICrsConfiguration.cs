using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public interface ICrsConfiguration
    {
        ICrsConfiguration AddAmbientValues(Action<IAmbientValuesRegistration> ambientValuesConfiguration);

        ICrsConfiguration AddCommands(Action<ICommandRegistry> registryConfiguration);

        ICrsConfiguration AddEndpoints(Action<ICrsEndpointConfigurationRoot> endpointConfiguration);

    }

}
