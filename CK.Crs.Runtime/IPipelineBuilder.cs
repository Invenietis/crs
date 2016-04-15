using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime.Pipeline;

namespace CK.Crs.Runtime
{
    public interface IPipelineBuilder
    {
        /// <summary>
        /// Gets all registered components in the pipeline
        /// </summary>
        IEnumerable<Func<IPipeline, Task<IPipeline>>> Components { get; }
        
        /// <summary>
        /// Registers a component into the pipeline.
        /// </summary>
        /// <param name="inlineComponent"></param>
        /// <returns></returns>
        IPipelineBuilder Use( Func<IPipeline, Task<IPipeline>> inlineComponent );

        /// <summary>
        /// Clears all registered components in the pipeline.
        /// </summary>
        /// <returns></returns>
        IPipelineBuilder Clear();
    }
}
