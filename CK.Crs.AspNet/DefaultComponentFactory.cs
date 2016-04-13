using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs
{
    public class DefaultPipelineComponentFactory : IPipelineComponentFactory
    {
        public PipelineComponent CreateComponent( IPipeline pipeline, Type componentType )
        {
            return (PipelineComponent)ActivatorUtilities.CreateInstance( pipeline.CommandServices, componentType, pipeline );
        }
    }
}
