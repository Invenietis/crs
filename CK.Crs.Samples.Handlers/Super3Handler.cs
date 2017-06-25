using CK.Crs.Samples.Messages;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    public class Super3Handler : CommandHandler<Super3Command>
    {
        protected override Task DoHandleAsync(ICommandExecutionContext context, Super3Command command)
        {
            Console.WriteLine("Super 3");
            return Task.CompletedTask;
        }
    }
}
