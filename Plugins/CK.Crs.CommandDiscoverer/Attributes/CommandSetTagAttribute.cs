using System;

namespace CK.Crs.CommandDiscoverer.Attributes
{
    [AttributeUsage( AttributeTargets.Class )]
    public class CommandSetTagAttribute : Attribute
    {
        public string[] Traits { get; }

        public CommandSetTagAttribute( params string[] traits )
        {
            Traits = traits;
        }
    }
}
