using System;
using System.Globalization;

namespace CK.Crs
{
    public struct CommandName
    {
        private string _name;

        public CommandName( string name )
        {
            _name = name.ToLower( CultureInfo.InvariantCulture );
        }
        public CommandName( Type commandType )
        {
            var assemblyName = commandType.Assembly.GetName().Name;
            _name = (assemblyName + "-" + commandType.FullName).ToLower( CultureInfo.InvariantCulture );
        }

        public bool IsValid => !String.IsNullOrEmpty( _name );

        public static implicit operator string( CommandName name )
        {
            return name.ToString();
        }

        public static implicit operator CommandName( string name )
        {
            return new CommandName( name );
        }

        public override string ToString()
        {
            return _name;
        }
    }
}
