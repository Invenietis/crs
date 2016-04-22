using System.Reflection;
using System.Linq;

namespace CK.Crs.Runtime.Meta
{
    internal class CommandPropertyInfo
    {
        readonly PropertyInfo _propertyInfo;
        readonly IAmbientValuesRegistration _ambientValue;
        public CommandPropertyInfo( PropertyInfo e, IAmbientValuesRegistration ambientValue )
        {
            _propertyInfo = e;
            _ambientValue = ambientValue;
        }

        public string ParameterName => _propertyInfo.Name;

        public string ParameterType => _propertyInfo.PropertyType.Name;

        public bool IsAmbientValue => _ambientValue.AmbientValues.FirstOrDefault( e => e.Name == _propertyInfo.Name ) != null;
    }
}