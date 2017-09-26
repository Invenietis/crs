using CK.Core;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICrsConnectionManager
    {
        Task AddConnection( IActivityMonitor monitor, CallerId correlation);
        Task RemoveConnection( IActivityMonitor monitor, CallerId correlation );
    }

}
