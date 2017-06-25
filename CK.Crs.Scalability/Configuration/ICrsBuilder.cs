using CK.Crs.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace CK.Crs.Scalability
{
    public interface ICrsBuilder
    {
        /// <summary>
        /// Gets all registered services so far.
        /// </summary>
        IServiceProvider Services { get; }

        /// <summary>
        /// Configures the input
        /// </summary>
        ICommandReceiverConfigurationInput Input { get; }

        /// <summary>
        /// Configures the output
        /// </summary>
        /// <returns></returns>
        ICommandReceiverConfigurationOutput Output { get; }
    }
}
