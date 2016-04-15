using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class CommandRouteCollection : ICommandRouteCollection
    {
        readonly string _path;
        internal readonly Dictionary<CommandRoutePath, RoutedCommandDescriptor> RouteStorage;

        public string PathBase { get { return _path; } }

        public CommandRouteCollection( string path )
        {
            _path = path;
            RouteStorage = new Dictionary<CommandRoutePath, RoutedCommandDescriptor>( new CommandRoutePath.Comparer() );
        }

        public RoutedCommandDescriptor FindCommandDescriptor( string path )
        {
            RoutedCommandDescriptor v;
            var key = new CommandRoutePath( path );
            RouteStorage.TryGetValue( key, out v );
            return v;
        }

        public RoutedCommandDescriptor AddCommandRoute( CommandDescription descriptor )
        {
            var p = new CommandRoutePath(_path, descriptor.Name);
            // Overrides...
            return RouteStorage[p] = new RoutedCommandDescriptor( p, descriptor );
        }
    }
}
