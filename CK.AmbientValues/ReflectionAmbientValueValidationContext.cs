using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using CK.Core;

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

        public sealed override async Task<bool> ValidateValue( string valueName, IEqualityComparer<object> comparer )
        {
            var actionType = Action.GetType();
            var properties = actionType.GetProperties().ToDictionary( k => k.Name );
            var property = properties.GetValueWithDefault( valueName, null );
            if( property == null )
            {
                Monitor.Info( $"Property {valueName} not found on command { actionType.Name}" );
                return true;
            }

            var value = property.GetMethod.Invoke( Action, null );
            Monitor.Trace( $"Getting {property.Name} by reflection on the command and obtained {value?.ToString()}." );
            IComparable ambientValue = await AmbientValues.GetValueAsync( valueName );
            Monitor.Trace( $"Getting {valueName} in the ambient values and obtained {ambientValue?.ToString()}." );
            return comparer.Equals( value, ambientValue );
        }
    }
}
