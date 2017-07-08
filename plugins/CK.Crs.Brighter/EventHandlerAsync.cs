using Paramore.Brighter;
using System.Threading.Tasks;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public abstract class EventHandlerAsync<T> : RequestHandlerAsync<T>, IRequestHandler<T> where T : class, IEvent
    {
        public Task HandleAsync( T request, ICommandContext requestContext) => HandleEventAsync( request, requestContext);

        public async override Task<T> HandleAsync(T command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var mon = (IActivityMonitor)Context.Bag.GetValueWithDefaultFunc("CK.Crs.ActivityMonitor", k => new ActivityMonitor());
            var callerId = (string)Context.Bag.GetValueWithDefault("CK.Crs.CallerId", null);

            var ctx = new CommandContext(command.Id, mon, callerId, cancellationToken);
            await HandleEventAsync(command, ctx);
            
            return await base.HandleAsync(command, cancellationToken);
        }

        protected abstract Task HandleEventAsync(T evt, ICommandContext context);
    }
}
