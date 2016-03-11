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
            var value = await valueProvider.GetValueAsync( this );
            
            var disposable = valueProvider as IDisposable;
            if( disposable != null ) disposable.Dispose();

            return (T)value;
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

    public class DefaultAmbientValueFactory : IAmbientValueProviderFactory
    {
        IServiceProvider _serviceProvider;
        public DefaultAmbientValueFactory( IServiceProvider serviceProvider )
        {
            _serviceProvider = serviceProvider;
        }
        public virtual IAmbientValueProvider Create( Type providerType )
        {
            return _serviceProvider.GetService( providerType ) as IAmbientValueProvider;
        }
    }
}
