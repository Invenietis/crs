using System;

namespace CK.Crs
{
    public interface ICrsEndpointConfiguration
    {
        ICrsEndpointConfigurationRoot Apply( Func<CommandModel, bool> filter );
    }

}
