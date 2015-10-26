using System;

namespace CK.Infrastructure.Commands
{
    [AttributeUsage( AttributeTargets.Method | AttributeTargets.Class )]
    public abstract class HandlerAttributeBase : Attribute, IDecorator
    {
        public int Order { get; set; }

        public virtual void OnCommandExecuting( CommandExecutionContext ctx ) { }
        public virtual void OnException( CommandExecutionContext ctx ) { }
        public virtual void OnCommandExecuted( CommandExecutionContext ctx ) { }
    }
}
