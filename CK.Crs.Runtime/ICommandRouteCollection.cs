using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface ICommandRouteCollection
    {
        string PathBase { get; }

        RoutedCommandDescriptor FindCommandDescriptor( string path );

        RoutedCommandDescriptor AddCommandRoute( CommandDescription descriptor );
    }
}
