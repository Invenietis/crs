using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Routing
{
    public interface ICommandRouteCollection
    {
        CommandRoute FindRoute( string receiverPath, string requestPath );

        CommandRoute AddRoute( string receiverPath, CommandDescription descriptor );
    }
}
