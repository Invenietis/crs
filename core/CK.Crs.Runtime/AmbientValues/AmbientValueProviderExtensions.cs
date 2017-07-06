using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CK.Core
{
    public interface IAmbientValuesRegistrationFrom<T>
    {
        IAmbientValuesProviderConfiguration<T> Select<TProperty>(Expression<Func<T, TProperty>> propertyAccessor);
    }

    public interface IAmbientValuesProviderConfiguration<T>
    {
        IAmbientValuesRegistrationFrom<T> ProvidedBy<TProvider>() where TProvider : class, IAmbientValueProvider;
    }


    public static class AmbientValueProviderExtensions
    {

        public static IAmbientValuesRegistrationFrom<T> AddAmbientValueProviderFrom<T>(this IAmbientValuesRegistration @this)
        {
            return new AmbientValuesRegistrationFrom<T>(@this);
        }

        public static IAmbientValuesRegistration RegisterValue(this IAmbientValuesRegistration @this, string name, object value)
        {
            var directProvider = new DirectProvider(value);
            @this.AddLazyAmbientValueProvider(name, (services) => directProvider);
            return @this;
        }

        class DirectProvider : IAmbientValueProvider
        {
            public DirectProvider(object value)
            {
                Value = value;
            }

            private object Value { get; }

            public Task<object> GetValueAsync(IAmbientValues values)
            {
                return Task.FromResult(Value);
            }
        }
    }
}
