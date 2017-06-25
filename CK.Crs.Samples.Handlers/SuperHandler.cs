using CK.Crs.Samples.Messages;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class SuperHandler : CommandHandler<SuperCommand>
    {
        protected override Task DoHandleAsync(ICommandExecutionContext context, SuperCommand command)
        {
            Console.WriteLine("Super");
            return Task.CompletedTask;
        }
    }
}
