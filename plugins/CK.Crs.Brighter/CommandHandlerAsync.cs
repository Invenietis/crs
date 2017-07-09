﻿using Paramore.Brighter;
using System.Threading.Tasks;
using System.Threading;
using CK.Core;

namespace CK.Crs
{
    public abstract class CommandHandlerAsync<T> : RequestHandlerAsync<T>, IRequestHandler<T> where T : class, ICommand
    {
        public Task HandleAsync(T command, ICommandContext requestContext ) => HandleCommandAsync(command, requestContext);

        public async override Task<T> HandleAsync(T command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var mon = (IActivityMonitor)Context.Bag.GetValueWithDefaultFunc("CK.Crs.ActivityMonitor", k => new ActivityMonitor());
            var callerId = (string)Context.Bag.GetValueWithDefault("CK.Crs.CallerId", null);

            var ctx = new CK.Crs.CommandContext(command.Id, mon, callerId, cancellationToken);
            await HandleCommandAsync(command, ctx);
            
            return await base.HandleAsync(command, cancellationToken);
        }

        protected abstract Task HandleCommandAsync(T command, ICommandContext context);
    }
}