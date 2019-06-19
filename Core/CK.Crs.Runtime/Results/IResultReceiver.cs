using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Results
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
        Task ReceiveResult<T>( T result, ICommandContext context );

        /// <summary>
        /// Receives the error of the a command invokation.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        Task ReceiveError( Exception ex, ICommandContext context );
    }
}
