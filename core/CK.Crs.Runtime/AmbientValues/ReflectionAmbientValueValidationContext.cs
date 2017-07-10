using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using CK.Core;
using Microsoft.Extensions.Caching.Memory;

namespace CK.Core
{
    public class ReflectionAmbientValueValidationContext : AmbientValueValidationContext
    {
        public object Action { get; }

        public ReflectionAmbientValueValidationContext( object command, IActivityMonitor monitor, IAmbientValues ambientValues ) :
            base( monitor, ambientValues )
        {
            Action = command;
        }

        public sealed override async Task<bool> ValidateValue( string valueName, AmbientValueComparer comparer )
        {
            var actionType = Action.GetType();
            var properties = actionType.GetProperties().ToDictionary( k => k.Name );
            var property = properties.GetValueWithDefault( valueName, null );
            if( property == null )
            {
                Monitor.Info().Send("Property {0} not found for value {1} on command {2}", valueName, valueName, actionType.Name);
                return true;
            }

            Monitor.Trace().Send("Getting {0} by reflection on the command and obtained {1}.", property.Name, value != null ? value.ToString() : "<NULL>");
            Monitor.Trace().Send("Getting {0} in the ambient values and obtained {1}.", valueName, ambientValue != null ? ambientValue.ToString() : "<NULL>");
            IComparable value = (IComparable)property.GetMethod.Invoke( Action, null );
            IComparable ambientValue = await AmbientValues.GetValueAsync( valueName );
            return comparer( valueName, value, ambientValue );
        }
    }
}
