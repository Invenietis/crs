using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using CK.Crs.Runtime.Pipeline;

namespace CK.Crs
{
    public interface IPipelineComponentFactory
    {
        PipelineComponent CreateComponent( IPipeline pipeline, Type componentType );
    }
}
