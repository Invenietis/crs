using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandReceiver : ICommandReceiver
    {
        readonly ICommandFileWriter _fileWriter;
        readonly ICommandResponseDispatcher _eventDispatcher;
        readonly ICommandHandlerFactory _handlerFactory;
        readonly ICommandHandlerRegistry _registry;

        public DefaultCommandReceiver( ICommandResponseDispatcher eventDispatcher, ICommandFileWriter fileWriter, ICommandHandlerFactory handlerFactory, ICommandHandlerRegistry registry )
        {
            _eventDispatcher = eventDispatcher;
            _fileWriter = fileWriter;
            _handlerFactory = handlerFactory;
            _registry = registry;
        }

        public async Task<ICommandResponse> ProcessCommandAsync( ICommandRequest commandRequest )
        {
            var context = new CommandContext(_eventDispatcher, _handlerFactory);
            context.Request = commandRequest;
            context.Response = new CommandResponse
            {
                CommandId = Guid.NewGuid()
            };

            if( !_registry.IsRegisterd( commandRequest.CommandServerType ) )
            {
                context.Response.ResponseType = CommandResponseType.Error;
                context.Response.Payload = new InvalidOperationException( "Handler not found for command type " + context.Request.CommandServerType );
            }
            else
            {
                context.HandlerType = _registry.GetHandlerType( context.Request.CommandServerType );
                if( context.Request.IsLongRunning )
                {
                    context.Response.Payload = context.Request.CallbackId;
                    context.Response.ResponseType = CommandResponseType.Deferred; // Deferred
                    new Task( async ( state ) => await Process( state as ICommandContext ), context ).Start( TaskScheduler.Current );
                }
                else
                {
                    context.Response.ResponseType = CommandResponseType.Direct; // Direct
                    await Process( context );
                }
            }
            return context.Response;
        }


        static async Task Process( ICommandContext ctx )
        {
            try
            {
                ICommandHandler handler = ctx.HandlerFactory.Create( ctx.HandlerType );
                ctx.Response.Payload = await handler.HandleAsync( ctx.Request.Command );
                ctx.Response.ResponseType = CommandResponseType.Direct; // Event
            }
            catch( Exception ex )
            {
                ctx.Response.ResponseType = CommandResponseType.Error;
                ctx.Response.Payload = ex;
            }
            if( ctx.Request.IsLongRunning )
            {
                await ctx.EventDispatcher.DispatchAsync( ctx.Request.CallbackId, ctx.Response );
            }
        }

    }
}
