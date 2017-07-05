using CK.Crs.Samples.Messages;
using Paramore.Brighter;
using System;
using System.Threading.Tasks;
using System.Threading;

namespace CK.Crs.Samples.Handlers
{
    public class SuperHandler : RequestHandlerAsync<SuperCommand>
    {
        public override Task<SuperCommand> HandleAsync(SuperCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            Console.WriteLine("Super");
            return base.HandleAsync(command, cancellationToken);
        }
    }
}
