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
            var context = new CommandContext( commandRequest, Guid.NewGuid() );
            var handlerType = _registry.GetHandlerType( context.Request.CommandType );
            if( handlerType == null )
            {
                string msg = "Handler not found for command type " + context.Request.CommandType;
                context.CreateErrorResponse( msg );
            }
            else
            {
                ICommandRunner runner = context.PrepareHandling( handlerType, _handlerFactory, _eventDispatcher );
                await runner.ProcessAsync( context );

            }
            return context.Response;
        }
    }
}
