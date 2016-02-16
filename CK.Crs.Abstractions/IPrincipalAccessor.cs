using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IPrincipalAccessor
    {
        IPrincipal User { get; }
    }
}
