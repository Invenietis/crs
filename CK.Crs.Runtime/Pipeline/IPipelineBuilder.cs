using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Pipeline
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
        /// Registers a typed component into the pipeline.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IPipelineBuilder Use<T>() where T : PipelineComponent;

        /// <summary>
        /// Clears all registered components in the pipeline.
        /// </summary>
        /// <returns></returns>
        IPipelineBuilder Clear();
    }
}
