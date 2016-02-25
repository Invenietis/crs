using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Xunit.Abstractions;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;

namespace CK.Crs.Tests
{
    public class CommandWithEventsTest
    {
        class SimpleCommand { }
        class SimpleEvent { }

        ITestOutputHelper _output;
        public CommandWithEventsTest( ITestOutputHelper output )
        {
            _output = output;
        }
        [Fact]
        public async Task an_event_emitter_should_be_set_when_events_are_published()
        {
            SimpleCommand command = new SimpleCommand();
            CommandExecutionContext ctx = new CommandExecutionContext( TestHelper.Monitor(_output.WriteLine ), command, Guid.NewGuid(), false, String.Empty, default( CancellationToken));
            try
            {
                await ctx.PublishEventAsync( new SimpleEvent() );
            }
            catch( Exception ex )
            {
                Assert.That( ex.GetBaseException(), Is.InstanceOf<InvalidOperationException>() );
            }
        }

        [Fact]
        public async Task when_an_event_emitter_is_configured_it_is_used_for_every_published_events()
        {
            SimpleCommand command = new SimpleCommand();
            CommandExecutionContext ctx = new CommandExecutionContext( TestHelper.Monitor(_output.WriteLine), command, Guid.NewGuid(), false, String.Empty, default( CancellationToken));

            bool eventEmitted=false;
            CommandExecutionContext.SetEventEmitter( e =>
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject( e );
                _output.WriteLine( json );
                eventEmitted = true; 
                return Task.FromResult<object>( null );
            } );
            Assert.That( eventEmitted, Is.False );
            await ctx.PublishEventAsync( new SimpleEvent() );
            Assert.That( eventEmitted, Is.True );
        }
    }
}
