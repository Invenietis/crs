using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CK.Core
{
    public interface IAmbientValuesRegistrationFrom<T>
    {
        IAmbientValuesProviderConfiguration<T> Select<TProperty>( Expression<Func<T, TProperty>> propertyAccessor ) where TProperty : IComparable;
    }

    public interface IAmbientValuesProviderConfiguration<T>
    {
        IAmbientValuesRegistrationFrom<T> ProvidedBy<TProvider>() where TProvider : class, IAmbientValueProvider;
    }


    public static class AmbientValueProviderExtensions
    {

        public static IAmbientValuesRegistrationFrom<T> AddProviderFrom<T>( this IAmbientValuesRegistration @this )
        {
            return new AmbientValuesRegistrationFrom<T>( @this );
        }

        public static IAmbientValuesRegistration RegisterValue( this IAmbientValuesRegistration @this, string name, IComparable value )
        {
            var directProvider = new DirectProvider( value );
            @this.AddProvider( name, ( services ) => directProvider );
            return @this;
        }

        class DirectProvider : IAmbientValueProvider
        {
            public DirectProvider( IComparable value )
            {
                Value = value;
            }

            private IComparable Value { get; }

            public Task<IComparable> GetValueAsync( IAmbientValues values )
            {
                return Task.FromResult( Value );
            }
        }
    }
}
