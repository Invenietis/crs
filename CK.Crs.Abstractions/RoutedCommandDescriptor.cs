using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class RoutedCommandDescriptor
    {
        public RoutedCommandDescriptor( CommandRoutePath route, CommandDescription commandDescriptor )
        {
            Route = Route;
            Descriptor = commandDescriptor;
            Filters = Core.Util.Array.Empty<Type>();
        }

        /// <summary>
        /// Gets the <see cref="CommandDescription"/> for this <see cref="RoutedCommandDescriptor"/>
        /// </summary>
        public CommandDescription Descriptor { get; }

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
