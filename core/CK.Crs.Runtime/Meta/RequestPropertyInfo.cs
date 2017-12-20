using System.Reflection;
using System.Linq;
using CK.Core;

namespace CK.Crs
{
    public sealed class RequestPropertyInfo
    {
        readonly PropertyInfo _propertyInfo;
        readonly IAmbientValuesRegistration _ambientValue;

        public RequestPropertyInfo( PropertyInfo e, IAmbientValuesRegistration ambientValue )
        {
            _propertyInfo = e;
            _ambientValue = ambientValue;
        }

        public string ParameterName => _propertyInfo.Name;

        public string ParameterType => _propertyInfo.PropertyType.Name;

        public bool IsAmbientValue => _ambientValue == null ?
            false :
            _ambientValue.AmbientValues.FirstOrDefault( e => e.Name == _propertyInfo.Name ) != null;
    }
}
