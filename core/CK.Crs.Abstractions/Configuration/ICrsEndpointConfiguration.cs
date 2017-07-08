using System;

namespace CK.Crs
{
    public interface ICrsEndpointConfiguration
    {
        ICrsEndpointConfigurationRoot Apply(Func<RequestDescription, bool> filter);
    }

}
