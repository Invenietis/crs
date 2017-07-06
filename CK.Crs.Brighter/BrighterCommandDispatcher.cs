using Paramore.Brighter;
using System.Threading.Tasks;
using System;

namespace CK.Crs
{
    public class BrighterCommandDispatcher : ICommandDispatcher
    {
        readonly IAmACommandProcessor _processor;

        public BrighterCommandDispatcher(IAmACommandProcessor processor)
        {
            _processor = processor;
        }

        Task ICommandDispatcher.PublishAsync<T>(T evt, ICommandContext context)
        {
            return _processor.PublishAsync( (dynamic)evt, false, context.CommandAborted ).ConfigureAwait(false);
        }

        async Task<object> ICommandDispatcher.SendAsync<T>(T command, ICommandContext context)
        {
            await _processor.SendAsync((dynamic)command, false, context.CommandAborted).ConfigureAwait( false );
            return Task.FromResult<object>( null );
        }
    }
}
