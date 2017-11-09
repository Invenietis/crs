using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Core
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

        public IEnumerable<IAmbientValueProviderDescriptor> Descriptors => _registration.AmbientValues;

        public bool IsDefined( string name )
        {
            return _registration.GetByName( name ) != null;
        }

        public async Task<IComparable> GetValueAsync( string name )
        {
            var ambientValueDescriptor = _registration.GetByName( name );
            if( ambientValueDescriptor == null )
                throw new ArgumentException( $"No AmbientValueProvider registered for {name}." );

            var provider = _factory.Create( ambientValueDescriptor );
            if( provider == null )
                throw new InvalidOperationException( $"Unable to create an instance of {ambientValueDescriptor.Name}." );

            var value = await provider.GetValueAsync( this );
            if( ambientValueDescriptor is IDisposable disposable ) disposable.Dispose();

            if( value != null ) return value;
            return default( IComparable );
        }
    }
}
