using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public class AmbientValues : IAmbientValues
    {
        IDictionary<string, Func<IAmbientValueProvider>> _lazyBag;

        readonly IAmbientValueProviderFactory _factory;
        public AmbientValues( IAmbientValueProviderFactory factory )
        {
            _factory = factory;
        }

        public bool IsDefined( string name )
        {
            if( _lazyBag == null ) return false;
            return _lazyBag.Keys.Contains( name );
        }

        public async Task<T> GetValueAsync<T>( string name )
        {
            if( !IsDefined( name ) ) return default( T );
            var valueProvider = _lazyBag[name]();
            if( valueProvider == null )
                throw new ArgumentException( $"Invalid valueProvider for {name}." );
            var value = await valueProvider.GetValueAsync( this );

            var disposable = valueProvider as IDisposable;
            if( disposable != null ) disposable.Dispose();

            if( value != null ) return (T)value;
            return default( T );
        }

        public void Register( string key, Func<IAmbientValueProvider> provider )
        {
            if( _lazyBag == null ) _lazyBag = new Dictionary<string, Func<IAmbientValueProvider>>();
            _lazyBag.Add( key, provider );
        }

        public void Register<T>( string key ) where T : IAmbientValueProvider
        {
            if( _lazyBag == null ) _lazyBag = new Dictionary<string, Func<IAmbientValueProvider>>();
            _lazyBag.Add( key, () => _factory.Create( typeof( T ) ) );
        }
    }
}
