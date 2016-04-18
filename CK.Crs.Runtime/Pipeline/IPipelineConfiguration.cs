using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Pipeline
{
    public interface IPipelineConfiguration
    {
        /// <summary>
        /// Gets the <see cref="ICommandRouteCollection"/> 
        /// </summary>
        ICommandRouteCollection Routes { get; }

        /// <summary>
        /// Gets the <see cref="PipelineEvents"/> 
        /// </summary>
        PipelineEvents Events { get; }
    }

}
