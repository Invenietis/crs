using Paramore.Brighter;
using System.Threading.Tasks;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public abstract class CommandHandlerAsync<T> : RequestHandlerAsync<T>, ICommandHandler<T> where T : class, IRequest
    {
        public async Task<object> HandleAsync(ICommandContext commandContext, T command)
        {
            var result = await HandleCommandAsync(command, commandContext);
            // Dispatch ?
            return result;
        }

        public async override Task<T> HandleAsync(T command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var mon = (IActivityMonitor)Context.Bag.GetValueWithDefaultFunc("CK.Crs.ActivityMonitor", k => new ActivityMonitor());
            var callerId = (string)Context.Bag.GetValueWithDefault("CK.Crs.CallerId", null);

            var ctx = new CommandContext(command.Id, mon, callerId, cancellationToken);
            var result = await HandleCommandAsync(command, ctx);
            // Dispatch ?
            return await base.HandleAsync(command, cancellationToken);
        }

        protected abstract Task<object> HandleCommandAsync(T command, ICommandContext context);
    }
}
