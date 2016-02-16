using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{

    public interface IMutableCommand
    {
        IActivityMonitor Monitor { get; }

        CancellationToken CommandAborted { get; }

        void Mutate( IMutableCommand command );
    }

}
