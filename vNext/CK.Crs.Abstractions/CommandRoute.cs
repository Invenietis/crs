using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    /// <summary>
    /// Describes a command registration in an CrsEndpoint.
    /// </summary>
    public class CommandRoute
    {
        /// <summary>
        /// Creates a <see cref="CommandRoute"/> from a path and a description
        /// </summary>
        /// <param name="route">The <see cref="CommandRoutePath"/> of the command</param>
        /// <param name="commandDescriptor">A <see cref="CommandDescription"/> of the command</param>
        public CommandRoute( CommandRoutePath route, CommandDescription commandDescriptor )
        {
            if( route == null ) throw new ArgumentNullException( "route" );
            if( commandDescriptor == null ) throw new ArgumentNullException( "commandDescriptor" );

            Route = route;
            Descriptor = commandDescriptor;
            Filters = Core.Util.Array.Empty<Type>();
        }

        /// <summary>
        /// Gets the <see cref="CommandDescription"/> for this <see cref="CommandRoute"/>
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
