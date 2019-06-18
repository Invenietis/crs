using System.Threading.Tasks;

namespace CK.Crs.Samples.AspNetCoreBasicApp.Commands
{
    public class CommandHandler :
        ICommandHandler<MySynchronousCommand, string>,
        ICommandHandler<MyDeferredCommand, string>
    {
        public Task<string> HandleAsync( MySynchronousCommand command, ICommandContext context )
        {
            return Task.FromResult( "OK" );
        }

        public Task<string> HandleAsync( MyDeferredCommand command, ICommandContext context )
        {
            return Task.FromResult( "OK" );
        }
    }
}
