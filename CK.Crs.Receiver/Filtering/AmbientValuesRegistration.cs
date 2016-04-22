using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Runtime.Filtering
{

    enum AmbientValueProviderType
    {
        Direct,
        Lazy
    }

    public class AmbientValuesRegistration : IAmbientValuesRegistration
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

        public void Register( string key, Func<IServiceProvider, IAmbientValueProvider> provider )
        {
            if( _lazyBag == null ) _lazyBag = new Dictionary<string, IAmbientValueProviderDescriptor>();
            _lazyBag.Add( key, new DirectAmbientValueDescriptor
            {
                Name = key,
                Resolver = provider
            } );
        }

        public void Register<T>( string key ) where T : class, IAmbientValueProvider
        {
            if( _lazyBag == null ) _lazyBag = new Dictionary<string, IAmbientValueProviderDescriptor>();
            _lazyBag.Add( key, new TypeAmbientValueDescriptor()
            {
                Name = key,
                AmbientValueType = typeof( T )
            } );
            _services.AddScoped<T>();
        }

        public IAmbientValueProviderDescriptor GetByName( string name )
        {
            if( _lazyBag == null ) return null;
            IAmbientValueProviderDescriptor f = null;
            _lazyBag.TryGetValue( name, out f );
            return f;
        }

        internal class TypeAmbientValueDescriptor : IAmbientValueProviderDescriptor
        {
            internal Type AmbientValueType { get; set; }

            public string Name { get; set; }

            internal AmbientValueProviderType ProviderType { get; set; }

            public IAmbientValueProvider Resolve( IServiceProvider services )
            {
                return services.GetService( AmbientValueType ) as IAmbientValueProvider;
            }
        }

        internal class DirectAmbientValueDescriptor : IAmbientValueProviderDescriptor
        {
            public string Name { get; set; }

            internal AmbientValueProviderType ProviderType { get; set; }

            internal Func<IServiceProvider, IAmbientValueProvider> Resolver { get; set; }

            public IAmbientValueProvider Resolve( IServiceProvider services ) => Resolver( services );
        }
    }
}
