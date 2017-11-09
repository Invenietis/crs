using System;
using System.Reflection;

namespace CK.Core
{
    internal class AmbientValuesProviderConfiguration<T> : IAmbientValuesProviderConfiguration<T>
    {
        private PropertyInfo _propInfo;
        private Action<IConfigurableAmbientValueProviderDescriptor> _configAction;
        readonly IAmbientValuesRegistration _registration;
        readonly IAmbientValuesRegistrationFrom<T> _from;

        public AmbientValuesProviderConfiguration( IAmbientValuesRegistration registration, IAmbientValuesRegistrationFrom<T> from, PropertyInfo propInfo )
        {
            _registration = registration;
            _from = from;
            _propInfo = propInfo;
        }

        public IAmbientValuesProviderConfiguration<T> Configure( Action<IConfigurableAmbientValueProviderDescriptor> configure )
        {
            _configAction = configure;
            return this;
        }

        IAmbientValuesRegistrationFrom<T> IAmbientValuesProviderConfiguration<T>.ProvidedBy<TProvider>()
        {
            _registration.AddProvider<TProvider>( _propInfo.Name, _configAction );
            return _from;
        }
    }
}
