using System;

namespace CK.Crs.CommandDiscoverer.Attributes
{
    [AttributeUsage( AttributeTargets.Class )]
    public class CommandCustomBinderAttribute : Attribute
    {
        public ICommandBinder CommandBinder { get; }

        public CommandCustomBinderAttribute( ICommandBinder commandBinder )
        {
            CommandBinder = commandBinder;
        }
    }
}
