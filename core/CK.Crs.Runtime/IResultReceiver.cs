using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// A simple result receiver definition.
    /// </summary>
    public interface IResultReceiver
    {
        /// <summary>
        /// A friendly name for the receivers
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Receives the result for the given <see cref="ICommandContext"/>
        /// </summary>
        /// <param name="result"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ReceiveResult( object result, ICommandContext context );
    }
}
