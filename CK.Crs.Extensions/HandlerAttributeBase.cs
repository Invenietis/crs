using System;

namespace CK.Crs
{
    [AttributeUsage( AttributeTargets.Method | AttributeTargets.Class )]
    public abstract class HandlerAttributeBase : Attribute, ICommandDecorator
    {
        public int Order { get; set; }
        public virtual void OnCommandExecuting( CommandContext ctx ) { }
        public virtual void OnException( CommandContext ctx ) { }
        public virtual void OnCommandExecuted( CommandContext ctx ) { }
    }
}
