using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs.Runtime
{
    public abstract class CommandResponse
    {
        internal CommandResponse( Guid commandId )
        {
            CommandId = commandId;
        }

        public CommandResponseType ResponseType { get; protected set; }

        public object Payload { get; protected set; }

        public Guid CommandId { get; private set; }

        internal static CommandResponse CreateFromContext( CommandContext context )
        {
            if( context.IsDirty )
            {
                if( context.Exception != null )
                    return new CommandErrorResponse( context.Exception.Message, context.ExecutionContext.CommandId );

                if( context.Result != null )
                {
                    if( context.Result is ValidationResult )
                        return new CommandInvalidResponse( context.ExecutionContext, context.Result );

                    return new CommandResultResponse( context.Result, context.ExecutionContext );
                }
            }

            return new CommandDeferredResponse( context.ExecutionContext );
        }
    }

}
