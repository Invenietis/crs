using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public interface IAmbientValueProviderDescriptor
    {
        string Name { get; }

        IAmbientValueProvider Resolve( IServiceProvider services );
    }
}
