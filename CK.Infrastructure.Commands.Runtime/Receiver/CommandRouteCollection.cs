using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class CommandRouteCollection : ICommandRouteCollection
    {
        readonly string _path;
        readonly Dictionary<CommandRoutePath, RoutedCommandDescriptor> _routeStorage;

        public string PathBase { get { return _path; } }

        public CommandRouteCollection( string path )
        {
            _path = path;
            _routeStorage = new Dictionary<CommandRoutePath, RoutedCommandDescriptor>();
        }

        public RoutedCommandDescriptor FindCommandDescriptor( string path )
        {
            RoutedCommandDescriptor v;
            var key = new CommandRoutePath( PathBase + path );
            _routeStorage.TryGetValue( key, out v );
            return v;
        }

        public RoutedCommandDescriptor AddCommandRoute( CommandDescriptor descriptor )
        {
            var p = new CommandRoutePath(_path, descriptor.Name);
            // Overrides...
            return _routeStorage[p] = new RoutedCommandDescriptor( p, descriptor );
        }
    }
}
