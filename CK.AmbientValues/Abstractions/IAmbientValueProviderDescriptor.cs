using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Core
{
    public interface IAmbientValueProviderDescriptor
    {
        string Name { get; }

        object GetMetadata( Type type );

        IAmbientValueProvider Resolve( IServiceProvider services );
    }

    public interface IConfigurableAmbientValueProviderDescriptor
    {
        IConfigurableAmbientValueProviderDescriptor AddMetadata<T>( T value ) where T : class;
    }
}
