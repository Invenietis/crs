using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public abstract class CommandResponse : ICommandResponse
    {
        internal CommandResponse( Guid commandId )
        {
            CommandId = commandId;
        }

        public CommandResponseType ResponseType { get; protected set; }

        public object Payload { get; protected set; }

        public Guid CommandId { get; private set; }

        ///// <summary>
        ///// Mutates the context by creating a <see cref="ICommandResponse"/>. 
        ///// </summary>
        ///// <returns></returns>
        //public static void SetResponse( CommandExecutionContext ctx )
        //{
        //    if( ctx.IsResponseCreated )
        //    {
        //        if( ctx.RuntimeContext.IsLongRunning )
        //        {
        //            ctx.Response = new CommandResultResponse( ctx.Response, ctx.RuntimeContext );
        //        }

        //        throw new InvalidOperationException( "There is already a Response created for this CommandContext." );
        //    }

        //    if( ctx.Exception != null ) ctx.Response = new CommandErrorResponse( ctx.Exception.Message, ctx.RuntimeContext.CommandId );
        //    else
        //    {
        //        if( ctx.RuntimeContext.IsLongRunning ) ctx.Response = new CommandDeferredResponse( ctx.RuntimeContext );
        //        else ctx.Response = new CommandResultResponse( ctx.Response, ctx.RuntimeContext );
        //    }
        //}

    }

}
