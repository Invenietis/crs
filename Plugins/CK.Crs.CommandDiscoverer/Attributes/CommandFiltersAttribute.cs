using System;

namespace CK.Crs.CommandDiscoverer.Attributes
{
    [AttributeUsage( AttributeTargets.Class )]
    public class CommandFiltersAttribute : Attribute
    {
        public Type[] Filters { get; }

        public CommandFiltersAttribute( params Type[] filters )
        {
            Filters = filters;
        }
    }
}
