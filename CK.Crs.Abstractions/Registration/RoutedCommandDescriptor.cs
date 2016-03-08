using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class RoutedCommandDescriptor
    {
        public RoutedCommandDescriptor( CommandRoutePath route, CommandDescriptor commandDescriptor )
        {
            Route = Route;
            Descriptor = commandDescriptor;
            Filters = Core.Util.Array.Empty<Type>();
        }

        /// <summary>
        /// Gets the <see cref="CommandDescriptor"/> for this <see cref="RoutedCommandDescriptor"/>
        /// </summary>
        public CommandDescriptor Descriptor { get; }

        /// <summary>
        /// Gets the Route of this command
        /// </summary>
        public CommandRoutePath Route { get; }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyCollection<Type> Filters { get; set; }
    }
}
