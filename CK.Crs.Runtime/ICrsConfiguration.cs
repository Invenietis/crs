using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public interface ICrsConfiguration
    {
        /// <summary>
        /// Gets the receiver path.
        /// </summary>
        string ReceiverPath { get; }
        
        /// <summary>
        /// Gets the <see cref="ICommandRouteCollection"/> 
        /// </summary>
        IReadOnlyDictionary<CommandRoutePath, CommandRoute> Routes { get; }

        /// <summary>
        /// Gets the <see cref="PipelineEvents"/> 
        /// </summary>
        PipelineEvents Events { get; }

        IPipelineBuilder Pipeline { get; }
    }

}
