using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace CK.Crs
{
    public static class ReflectionUtil
    {
        public static bool IsAssignableToGenericType( Type givenType, Type type )
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach( var it in interfaceTypes )
            {
                if( it.GetTypeInfo().IsGenericType )
                {
                    if( it.GetGenericTypeDefinition() == type ) return true;
                }
                else
                {
                    if( it == type ) return true;
                }
            }

            if( givenType.GetTypeInfo().IsGenericType && givenType.GetGenericTypeDefinition() == type )
                return true;

            Type baseType = givenType.GetTypeInfo().BaseType;
            if( baseType == null ) return false;

            return IsAssignableToGenericType( baseType, type );
        }

    }
}
