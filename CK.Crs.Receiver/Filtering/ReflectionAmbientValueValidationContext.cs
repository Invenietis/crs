using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using CK.Core;
using Microsoft.Extensions.Caching.Memory;

namespace CK.Crs.Runtime.Filtering
{
    class ReflectionAmbientValueValidationContext : AmbientValueValidationContext
    {
        readonly IMemoryCache _typeCache;
        public ReflectionAmbientValueValidationContext(IActivityMonitor monitor, CommandAction action, IAmbientValues ambientValues, IMemoryCache typeCache) :
            base(monitor, action, ambientValues)
        {
            _typeCache = typeCache;
        }

        public sealed override async Task<bool> ValidateValue<T>(string valueName, AmbientValueComparer<T> comparer)
        {
            var properties = _typeCache.GetOrCreate<IDictionary<string, PropertyInfo>>(Action.Description.CommandType.FullName, e =>
            {
                return Action.Description.CommandType.GetProperties().ToDictionary(k => k.Name);
            });
            var property = properties.GetValueWithDefault(valueName, null);
            if (property == null)
            {
                Monitor.Info().Send("Property {0} not found for value {1} on command {2}", valueName, valueName, Action.Description.CommandType.Name);
                return true;
            }

            T value = (T)property.GetMethod.Invoke(Action.Command, null);
            Monitor.Trace().Send("Getting {0} by reflection on the command and obtained {1}.", property.Name, value != null ? value.ToString() : "<NULL>");
            T ambientValue = await AmbientValues.GetValueAsync<T>(valueName);
            Monitor.Trace().Send("Getting {0} in the ambient values and obtained {1}.", valueName, ambientValue != null ? ambientValue.ToString() : "<NULL>");
            return comparer(valueName, value, ambientValue);
        }
    }
}
