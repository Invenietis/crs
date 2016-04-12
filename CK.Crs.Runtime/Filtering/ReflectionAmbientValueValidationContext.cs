using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using CK.Core;

namespace CK.Crs.Runtime.Pipeline
{
    class ReflectionAmbientValueValidationContext : AmbientValueValidationContext
    {
        public IReadOnlyCollection<PropertyInfo> Properties { get; }

        public ReflectionAmbientValueValidationContext( IActivityMonitor monitor, CommandAction action, IAmbientValues ambientValues ) : base( monitor, action, ambientValues )
        {
            Properties = action.Description.Descriptor.CommandType.GetProperties().ToArray();
        }

        public sealed override async Task<bool> ValidateValue<T>( string valueName, AmbientValueComparer<T> comparer )
        {
            var property = Properties.FirstOrDefault( x => x.Name == valueName);
            if( property == null ) Monitor.Info().Send( "Property {0} not found for value {1} on command {2}", valueName, valueName, Action.Description.Descriptor.CommandType.Name );
            else
            {
                T value = (T) property.GetMethod.Invoke( Action.Command, null );
                Monitor.Trace().Send( "Getting {0} by reflection on the command and obtained {1}.", property.Name, value != null ? value.ToString() : "<NULL>" );
                T ambientValue = await AmbientValues.GetValueAsync<T>( valueName);
                Monitor.Trace().Send( "Getting {0} in the ambient values and obtained {1}.", valueName, ambientValue != null ? ambientValue.ToString() : "<NULL>" );
                return comparer( valueName, value, ambientValue );
            }
            return true;
        }
    }
}
