using System;

namespace CK.Crs
{
    public interface IRequestServicesCommandFeature
    {
        IServiceProvider RequestServices { get; }
    }
}
