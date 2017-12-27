using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs
{
    public interface IRequestServicesCommandFeature
    {
        IServiceProvider RequestServices { get; }
    }
}
