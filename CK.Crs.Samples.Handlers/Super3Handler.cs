using CK.Crs.Samples.Messages;
using System;
using System.Threading.Tasks;

namespace CK.Crs.Samples.Handlers
{
    class Super3CommandExecutedEvent
    {

    }

    public class Super3Handler : CommandHandler<Super3Command>
    {
        protected override Task DoHandleAsync(ICommandExecutionContext context, Super3Command command)
        {
            Console.WriteLine("Super 3");
            
            //context.ExternalEvents?.Push(context.Monitor, new Super3CommandExecutedEvent());
            return Task.CompletedTask;
        }
    }
}
