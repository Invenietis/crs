using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandRouteCollection
    {
        IEnumerable<CommandRoute> All { get; }

        CommandRoute FindRoute( string receiverPath, string requestPath );

        CommandRoute AddRoute( string receiverPath, CommandDescription descriptor );
    }
}
