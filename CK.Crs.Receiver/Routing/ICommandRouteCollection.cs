using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Routing
{
    interface ICommandRouteCollection
    {
        CommandRoute FindRoute( string receiverPath, string requestPath );

        CommandRoute AddRoute( string receiverPath, CommandDescription descriptor );
    }
}
