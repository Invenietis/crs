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

        public CommandRouteCollection( string path )
        {
            _path = path;
            _routeStorage = new Dictionary<CommandRoutePath, RoutedCommandDescriptor>();
        }

        public RoutedCommandDescriptor FindCommandDescriptor( string path )
        {
            RoutedCommandDescriptor v;
            var key = new CommandRoutePath( path );
            _routeStorage.TryGetValue( key, out v );
            return v;
        }

        public void AddCommandRoute( CommandDescriptor descriptor )
        {
            var p = new CommandRoutePath(_path, descriptor.Name);
            // Overrides...
            _routeStorage[p] = new RoutedCommandDescriptor( p, descriptor );
        }
    }
}
