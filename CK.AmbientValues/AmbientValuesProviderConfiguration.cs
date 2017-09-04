using System;
using System.Reflection;

namespace CK.Core
{
    internal class AmbientValuesProviderConfiguration<T> : IAmbientValuesProviderConfiguration<T>
    {
        private PropertyInfo _propInfo;
        readonly IAmbientValuesRegistration _registration;
        readonly IAmbientValuesRegistrationFrom<T> _from;

        public AmbientValuesProviderConfiguration(  IAmbientValuesRegistration registration, IAmbientValuesRegistrationFrom<T> from, PropertyInfo propInfo )
        {
            _registration = registration;
            _from = from;
            _propInfo = propInfo;
        }

        IAmbientValuesRegistrationFrom<T> IAmbientValuesProviderConfiguration<T>.ProvidedBy<TProvider>()
        {
            _registration.AddAmbientValueProvider<TProvider>(_propInfo.Name);
            return _from;
        }
    }
}