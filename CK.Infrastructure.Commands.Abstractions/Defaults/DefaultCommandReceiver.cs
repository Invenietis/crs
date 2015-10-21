using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Infrastructure.Commands.Framework;

namespace CK.Infrastructure.Commands
{
    public class DefaultCommandReceiver : ICommandReceiver, ICommandHandlerRegistry
    {
        readonly ICommandFileWriter _fileWriter;
        readonly IEventDispatcher _eventDispatcher;
        readonly ICommandHandlerFactory _handlerFactory;
        Dictionary<Type, Type> HandlerMap { get; } = new Dictionary<Type, Type>();

        public DefaultCommandReceiver( IEventDispatcher eventDispatcher, ICommandFileWriter fileWriter, ICommandHandlerFactory handlerFactory )
        {
            _eventDispatcher = eventDispatcher;
            _fileWriter = fileWriter;
            _handlerFactory = handlerFactory;
        }

        public void RegisterHandler<T, THandler>()
        {
            if( HandlerMap.ContainsKey( typeof( T ) ) )
                throw new ArgumentException( "An handler is already registered for this command " + typeof( T ).FullName );

            HandlerMap.Add( typeof( T ), typeof( THandler ) );
        }

        public async Task<ICommandResponse> ProcessCommandAsync( ICommandRequest commandRequest )
        {
            var context = new CommandContext(_eventDispatcher, _handlerFactory);
            context.Request = commandRequest;
            context.Response = new CommandResponse
            {
                CommandId = Guid.NewGuid()
            };

            if( !HandlerMap.ContainsKey( commandRequest.CommandServerType ) )
            {
                context.Response.ResponseType = CommandResponseType.Error;
                context.Response.Payload = new InvalidOperationException( "Handler not found for command type " + context.Request.CommandServerType );
            }
            else
            {
                context.IsAsynchronous = ShouldHandleAsynchronously( context.Request.CommandServerType );
                context.HandlerType = HandlerMap[context.Request.CommandServerType];
                if( context.IsAsynchronous )
                {
                    context.Response.Payload = new { context.Response.CommandId, context.Request.CallbackId };
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

        protected virtual bool ShouldHandleAsynchronously( Type commandServerType )
        {
            return commandServerType.GetCustomAttributes( typeof( Framework.AsyncCommandAttribute ), false ).Length > 0;
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
            if( ctx.IsAsynchronous )
            {
                await ctx.EventDispatcher.DispatchAsync( ctx.Request.CallbackId, ctx.Response );
            }
        }

    }
}
