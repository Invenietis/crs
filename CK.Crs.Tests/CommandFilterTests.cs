using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace CK.Infrastructure.Commands.Tests
{
    [TestFixture]
    public class CommandFilterTests
    {
        [Test]
        public async Task InternalFilters_Are_Always_Executed_Before_Custom_User_Filters()
        {
            var filter = Substitute.For<ICommandFilter>();
            await filter.OnCommandReceived( Arg.Any<CommandExecutionContext>() );

            var factories = Substitute.For<ICommandReceiverFactories>();
            factories.CreateFilter( Arg.Any<Type>() ).Returns( filter );

            CommandReceiver receiver = new CommandReceiver( Substitute.For<ICommandExecutorSelector>(), factories );

            var request = Substitute.For<ICommandRequest>();
            ICommandResponse response = await receiver.ProcessCommandAsync( request, new Core.ActivityMonitor() );

        }
    }
}
