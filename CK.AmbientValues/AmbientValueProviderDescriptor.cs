using System;
using System.Collections.Generic;
using System.Text;
using CK.Core;

namespace CK.Core
{
    public static class AmbientValueProviderDescriptorExtensions
    {
        public static T GetMetadata<T>( this IAmbientValueProviderDescriptor @this ) where T : class
        {
            return @this.GetMetadata( typeof( T ) ) as T;
        }
    }

    internal abstract class BaseAmbientValueProviderDescriptor : IAmbientValueProviderDescriptor, IConfigurableAmbientValueProviderDescriptor
    {
        readonly Dictionary<Type, object> _metadata;

        public BaseAmbientValueProviderDescriptor()
        {
            _metadata = new Dictionary<Type, object>();
        }

        public string Name { get; set; }

        public object GetMetadata( Type type )
        {
            return _metadata.TryGetValue( type, out object val ) ? val : null;
        }

        public abstract IAmbientValueProvider Resolve( IServiceProvider services );

        internal AmbientValueProviderMode ProviderMode { get; set; }

        public IConfigurableAmbientValueProviderDescriptor AddMetadata<T>( T value ) where T : class
        {
            var type = typeof(T);
            if( _metadata.ContainsKey( type ) ) throw new InvalidOperationException( "A metadata for this type is already registered." );
            _metadata.Add( type, value );
            return this;
        }
    }

    internal class AmbientValueProviderDescriptor : BaseAmbientValueProviderDescriptor
    {
        public override IAmbientValueProvider Resolve( IServiceProvider services ) => Resolver( services );

        internal Func<IServiceProvider, IAmbientValueProvider> Resolver { get; set; }
    }

    internal class AutoResolveAmbientValueProviderDescriptor<TProvider> : BaseAmbientValueProviderDescriptor
        where TProvider : class, IAmbientValueProvider
    {
        public override IAmbientValueProvider Resolve( IServiceProvider services ) => services.GetService<TProvider>();
    }
}
