using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Crs.Runtime;
using NSubstitute;
using NUnit.Framework;

namespace CK.Crs.Tests
{
    //[TestFixture]
    public class CommandFilterTests
    {
        //[Test]
        public async Task InternalFilters_Are_Always_Executed_Before_Custom_User_Filters()
        {
            var filter = Substitute.For<ICommandFilter>();
            await filter.OnCommandReceived( Arg.Any<CommandContext>() );

            var factories = Substitute.For<IFactories>();
            factories.CreateFilter( Arg.Any<Type>() ).Returns( filter );

            CommandReceiver receiver = new CommandReceiver( Substitute.For<IExecutionStrategySelector>(), factories );

            var request = Substitute.For<CommandRequest>();
            CommandResponse response = await receiver.ProcessCommandAsync( request, new Core.ActivityMonitor() );

        }
    }
}
