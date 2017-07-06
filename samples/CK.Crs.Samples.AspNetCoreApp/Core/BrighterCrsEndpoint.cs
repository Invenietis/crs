using Paramore.Brighter;
using System.Threading.Tasks;

namespace CK.Crs.Samples.AspNetCoreApp
{
    public abstract class BrighterCrsEndpoint<T> : ICrsEndpoint<T> where T : class, ICommand
    {
        readonly IAmACommandProcessor _processor;

        public BrighterCrsEndpoint(IAmACommandProcessor processor)
        {
            _processor = processor;
        }

        public virtual async Task<CommandResponse> ReceiveCommand(T command, string callbackId)
        {
            await _processor.SendAsync(command);

            return new CommandDeferredResponse(command.Id, callbackId);
        }
    }

}
