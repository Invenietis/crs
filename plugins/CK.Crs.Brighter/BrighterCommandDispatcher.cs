using Paramore.Brighter;
using System.Threading.Tasks;
using System;

namespace CK.Crs
{
    public class BrighterCommandDispatcher : IBus
    {
        readonly IAmACommandProcessor _processor;

        public BrighterCommandDispatcher(IAmACommandProcessor processor)
        {
            _processor = processor;
        }

        Task IEventPublisher.PublishAsync<T>(T evt, ICommandContext context)
        {
            return _processor.PublishAsync( (dynamic)evt, false, context.Aborted );
        }

        Task ICommandDispatcher.SendAsync<T>(T command, ICommandContext context)
        {
            return _processor.SendAsync((dynamic)command, false, context.Aborted).ConfigureAwait( false );
        }
    }
}
