using Paramore.Brighter;
using System.Threading.Tasks;

namespace CK.Crs.Samples.AspNetCoreApp
{
    [CrsControllerNameConvention] // This attribute will be removed and automatically configured by the Framwework.
    public abstract class BrighterCrsController<T> : ICrsEndpoint<T> where T : class, ICommand
    {
        readonly IAmACommandProcessor _processor;

        public BrighterCrsController(IAmACommandProcessor processor)
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
