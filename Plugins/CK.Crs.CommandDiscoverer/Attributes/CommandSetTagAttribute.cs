using System;

namespace CK.Crs.CommandDiscoverer.Attributes
{
    [AttributeUsage( AttributeTargets.Class )]
    public class CommandSetTagAttribute : Attribute
    {
        public string[] Tags { get; }

        public CommandSetTagAttribute( params string[] tags )
        {
            Tags = tags;
        }
    }
}
