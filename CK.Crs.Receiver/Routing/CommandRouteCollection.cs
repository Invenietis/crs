using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Routing
{
    public class CommandRouteCollection : ICommandRouteCollection
    {
        internal readonly Dictionary<CommandRoutePath, CommandRoute> RouteStorage;

        public CommandRouteCollection()
        {
            RouteStorage = new Dictionary<CommandRoutePath, CommandRoute>( new CommandRoutePath.Comparer() );
        }

        public CommandRoute FindRoute( string receiverPath, string path )
        {
            CommandRoute v = null;
            var key = new CommandRoutePath( receiverPath, path );
            if( key.IsPrefixedBy( receiverPath ) )
            {
                RouteStorage.TryGetValue( key, out v );
            }
            return v;
        }

        public CommandRoute AddRoute( string receiverPath, CommandDescription descriptor )
        {
            var p = new CommandRoutePath(receiverPath, descriptor.Name);
            // Overrides...
            return RouteStorage[p] = new CommandRoute( p, descriptor );
        }
    }
}
