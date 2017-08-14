using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{

    public abstract class CommandHandlerBase<T, TResult> : ICommandHandler<T, TResult>
    {
        public abstract Task<TResult> HandleAsync( T request, ICommandContext context );

        async Task<object> ICommandHandlerWithResult<T>.HandleAsync( T request, ICommandContext context )
        {
            var res = await HandleAsync( request, context );
            return res;
        }
    }
}
