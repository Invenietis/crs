using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Core
{
    public interface IOperationExecutor<T>
    {
        /// <summary>
        /// Executing an operation is explicitely synchronous: it must be as fast as possible.
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="operation"></param>
        void Execute( IActivityMonitor monitor, T operation );
    }

}
