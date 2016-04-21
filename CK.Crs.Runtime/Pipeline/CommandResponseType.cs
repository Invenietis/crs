using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public enum CommandResponseType
    {
        ValidationError =   'V',
        InternalError =     'I',
        Synchronous =       'S',
        Asynchronous =      'A',
        Meta =              'M'
    }
}
