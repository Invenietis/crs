using System;

namespace CK.Crs.CommandDiscoverer.Attributes
{
    [AttributeUsage( AttributeTargets.Class )]
    public class CommandNameAttribute : Attribute
    {
        public string Name { get; }

        public CommandNameAttribute( string name )
        {
            Name = name;
        }
    }
}
