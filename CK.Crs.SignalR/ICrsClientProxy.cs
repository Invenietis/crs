using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Crs.Runtime;

namespace CK.Crs.SignalR
{
    public interface ICrsClientProxy
    {
        Task OnCommandComplete( CommandResponse response );
    }
}
