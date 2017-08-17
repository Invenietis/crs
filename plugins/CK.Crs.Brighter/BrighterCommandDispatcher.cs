using Paramore.Brighter;
using System.Threading.Tasks;
using System;

namespace CK.Crs
{
    public class BrighterCommandDispatcher : IBus
    {
        readonly IAmACommandProcessor _processor;

        public BrighterCommandDispatcher( IAmACommandProcessor processor )
        {
            _processor = processor;
        }

        Task ICommandDispatcher.PostAsync<T>( T command, ICommandContext context )
        {
            return _processor.PostAsync( (dynamic)command, continueOnCapturedContext: false, cancellationToken: context.Aborted );
        }

        Task IEventPublisher.PublishAsync<T>( T evt, ICommandContext context )
        {
            return _processor.PublishAsync( (dynamic)evt, continueOnCapturedContext: false, cancellationToken: context.Aborted );
        }

        async Task<object> ICommandReceiver.ReceiveCommand<T>( T command, ICommandContext context )
        {
            await _processor.SendAsync( (dynamic)command, continueOnCapturedContext: false, cancellationToken: context.Aborted );
            return Task.FromResult<object>( null );
        }
    }
}
