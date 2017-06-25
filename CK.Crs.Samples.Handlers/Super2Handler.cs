using CK.Crs.Samples.Messages;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class Super2Handler : CommandHandler<Super2Command>
    {
        protected override Task DoHandleAsync(ICommandExecutionContext context, Super2Command command)
        {
            Console.WriteLine("Super Super");
            return Task.CompletedTask;
        }
    }
}
