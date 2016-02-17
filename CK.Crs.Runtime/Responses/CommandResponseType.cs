using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public enum CommandResponseType
    {
        ValidationFailed = -2,
        Error = -1,
        Direct = 0,
        Deferred = 1
    }
}
