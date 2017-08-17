using System;

namespace CK.Crs
{
    public interface ICrsEndpointConfigurationRoot
    {
        ICrsEndpointConfiguration For( Type endpoint );
    }

}
