using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public enum CommandResponseType
    {
        Error = -1,
        Direct = 0,
        Deferred = 1
    }
}
