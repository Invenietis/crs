using System.Reflection;
using System.Linq;

namespace CK.Crs.Runtime.Meta
{
    class CommandPropertyInfo
    {
        readonly PropertyInfo _propertyInfo;
        readonly IAmbientValuesRegistration _ambientValue;
        public CommandPropertyInfo( PropertyInfo e, IAmbientValuesRegistration ambientValue )
        {
            _propertyInfo = e;
            _ambientValue = ambientValue;
            ParameterName = _propertyInfo.Name;
            ParameterType = _propertyInfo.PropertyType.Name;
            IsAmbientValue = _ambientValue.AmbientValues.FirstOrDefault( x => x.Name == _propertyInfo.Name ) != null;
        }

        public readonly string ParameterName;

        public readonly string ParameterType;

        public readonly bool IsAmbientValue;
    }
}