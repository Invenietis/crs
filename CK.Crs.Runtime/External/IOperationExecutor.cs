using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Runtime
{
    /// <summary>
    /// Defines an executor of an operation. An operation can be anything executable by the underlying executor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOperationExecutor<T>
    {
        /// <summary>
        /// Executing an operation is explicitely synchronous: it must be as fast as possible.
        /// </summary>
        /// <param name="monitor">The <see cref="IActivityMonitor"/> that monitors the operation.</param>
        /// <param name="operation">An operation is any type</param>
        void Execute( IActivityMonitor monitor, T operation );
    }

}
