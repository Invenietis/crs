using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime.Filtering
{
    class AmbientValues : IAmbientValues
    {
        readonly IAmbientValuesRegistration _registration;
        readonly IAmbientValueProviderFactory _factory;
        public AmbientValues( IAmbientValuesRegistration registration, IAmbientValueProviderFactory factory )
        {
            _registration = registration;
            _factory = factory;
        }

        public bool IsDefined( string name )
        {
            return _registration.GetByName( name ) != null;
        }

        public async Task<T> GetValueAsync<T>( string name )
        {
            var ambientValueDescriptor = _registration.GetByName( name );
            if( ambientValueDescriptor == null )
                throw new ArgumentException( $"No AmbientValueProvider registered for {name}." );

            var provider = _factory.Create( ambientValueDescriptor );
            if( provider == null )
                throw new InvalidOperationException( $"Unable to create an instance of {ambientValueDescriptor.Name}." );

            var value = await provider.GetValueAsync( this );
            var disposable = ambientValueDescriptor as IDisposable;
            if( disposable != null ) disposable.Dispose();

            if( value != null ) return (T)value;
            return default( T );
        }



    }
}
