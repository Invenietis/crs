using System;

namespace CK.Crs
{
    public struct CommandName
    {
        private string _name;

        public CommandName( string name )
        {
            _name = name;
        }
        public CommandName( Type commandType )
        {
            var assemblyName = commandType.Assembly.GetName().Name;
            _name = assemblyName + "-" + commandType.FullName;
        }

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
