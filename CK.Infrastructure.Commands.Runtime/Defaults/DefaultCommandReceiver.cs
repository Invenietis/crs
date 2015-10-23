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
            var handlerType = _registry.GetHandlerType( context.Request.CommandServerType );
            if( handlerType == null )
            {
                string msg = "Handler not found for command type " + context.Request.CommandServerType;
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

    interface ICommandRunner
    {
        Task ProcessAsync( CommandContext ctx );
    }

    abstract class AbstractRunner : ICommandRunner
    {
        public ICommandHandlerFactory HandlerFactory { get; private set; }

        public AbstractRunner( ICommandHandlerFactory factory )
        {
            HandlerFactory = factory;
        }

        protected class JobResult
        {
            public object Result { get; set; }
            public Exception Exception { get; set; }
            public TimeSpan ElapsedTime { get; set; }
        }

        protected async virtual Task<JobResult> DoJob( Type handlerType, object command )
        {
            JobResult result = new JobResult();
            try
            {
                ICommandHandler handler = HandlerFactory.Create( handlerType );
                result.Result = await handler.HandleAsync( command );
            }
            catch( Exception ex )
            {
                result.Exception = ex;
            }
            return result;
        }

        public abstract Task ProcessAsync( CommandContext ctx );
    }

    class InProcessCommandRunner : AbstractRunner
    {
        public InProcessCommandRunner( ICommandHandlerFactory factory ) : base( factory )
        {
        }

        public override async Task ProcessAsync( CommandContext ctx )
        {
            JobResult result = await DoJob( ctx.HandlerType, ctx.Request.Command );
            if( result.Exception == null )
            {
                ctx.CreateDirectResponse( result.Result );
            }
            else ctx.CreateErrorResponse( result.Exception.Message );
        }
    }

    class DedicatedRunner : ICommandRunner
    {
        InProcessCommandRunner _localRunner;
        public ICommandResponseDispatcher EventDispatcher { get; private set; }

        public DedicatedRunner( ICommandHandlerFactory factory, ICommandResponseDispatcher dispatcher )
        {
            _localRunner = new InProcessCommandRunner( factory );
            EventDispatcher = dispatcher;
        }

        public Task ProcessAsync( CommandContext ctx )
        {
            var newContext = ctx.CloneContext();
            var t = new Task( async () =>
            {
                await _localRunner.ProcessAsync( newContext );
                await EventDispatcher.DispatchAsync( newContext.Request.CallbackId, newContext.Response );

            }, TaskCreationOptions.LongRunning );

            ctx.CreateDeferredResponse();
            t.Start( TaskScheduler.Current );
            return Task.FromResult( 0 );
        }
    }
}
