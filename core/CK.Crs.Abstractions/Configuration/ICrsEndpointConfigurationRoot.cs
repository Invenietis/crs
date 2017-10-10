using System;

namespace CK.Crs
{
    public interface ICrsEndpointConfigurationRoot
    {
        ICrsEndpointConfiguration Map( Type endpoint );
    }

}
