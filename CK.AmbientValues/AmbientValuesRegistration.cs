using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Core
{
    enum AmbientValueProviderMode
    {
        Direct,
        Lazy
    }

    class AmbientValuesRegistration : IAmbientValuesRegistration
    {
        readonly IServiceCollection _services;
        IDictionary<string, IAmbientValueProviderDescriptor> _lazyBag;

        public AmbientValuesRegistration( IServiceCollection services )
        {
            _services = services;
        }

        public IEnumerable<IAmbientValueProviderDescriptor> AmbientValues
        {
            get { return _lazyBag != null ? _lazyBag.Values : CK.Core.Util.Array.Empty<IAmbientValueProviderDescriptor>(); }
        }

        public IAmbientValuesRegistration AddProvider( string key, Func<IServiceProvider, IAmbientValueProvider> provider, Action<IConfigurableAmbientValueProviderDescriptor> configure = null )
        {
            if( _lazyBag == null ) _lazyBag = new Dictionary<string, IAmbientValueProviderDescriptor>();

            var desc = new AmbientValueProviderDescriptor()
            {
                Name = key,
                Resolver = provider
            };

            configure?.Invoke( desc );
            _lazyBag.Add( key, desc );

            return this;
        }

        /// <summary>
        /// Registers an <see cref="IAmbientValueProvider"/> and registers it into the DI Container.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        public IAmbientValuesRegistration AddProvider<T>( string key, Action<IConfigurableAmbientValueProviderDescriptor> configure = null ) where T : class, IAmbientValueProvider
        {
            if( _lazyBag == null ) _lazyBag = new Dictionary<string, IAmbientValueProviderDescriptor>();

            var desc = new AutoResolveAmbientValueProviderDescriptor<T>()
            {
                Name = key
            };

            configure?.Invoke( desc );
            _lazyBag.Add( key, desc );

            _services.AddTransient<T>();

            return this;
        }

        public IAmbientValueProviderDescriptor GetByName( string name )
        {
            if( _lazyBag == null ) return null;
            IAmbientValueProviderDescriptor f = null;
            _lazyBag.TryGetValue( name, out f );
            return f;
        }
    }
}
