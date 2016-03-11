using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IPipelineEvents
    {
        Func<IAmbientValues, Task> AmbientValuesInvalid { get; set; }
    }

}
