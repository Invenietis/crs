using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs
{

    public static class AmbientValueProviderExtensions
    {
        public static IAmbientValues RegisterValue( this IAmbientValues @this, string name, object value )
        {
            var directProvider = new DirectProvider( value );
            @this.Register( name, () => directProvider );
            return @this;
        }

        class DirectProvider : IAmbientValueProvider
        {
            public DirectProvider( object value )
            {
                Value = value;
            }

            private object Value { get; }

            public Task<object> GetValueAsync( IAmbientValues values )
            {
                return Task.FromResult( Value );
            }
        }
    }
}
