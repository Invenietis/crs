using NUnit.Framework;
using System;

namespace CK.Crs.Tests
{
    [TestFixture]
    public class CallerIdTests
    {
        [Test]
        public void caller_id_can_be_serialized()
        {
            string protocol = "SignalR";
            string[] values = new[]
            {
                "0123456789abcdef_-",
                "My user|name!",
            };
            CallerId ci = new CallerId( protocol, values );

            Assert.That( ci.IsValid, Is.True );
            Assert.That( ci.Protocol, Is.EqualTo( "SignalR" ) );
            Assert.That( ci.ToString(), Is.EqualTo( "SignalR|0123456789abcdef_-|My%20user%7Cname%21" ) );
        }

        [Test]
        public void caller_id_can_be_deserialized()
        {
            string token = @"SignalR|0123456789abcdef_-|My%20user%7Cname%21";

            CallerId ci = CallerId.Parse( token );

            Assert.That( ci.IsValid, Is.True );
            Assert.That( ci.Protocol, Is.EqualTo( "SignalR" ) );
            Assert.That( ci.Values.Length, Is.EqualTo( 2 ) );
            Assert.That( ci.Values[0], Is.EqualTo( "0123456789abcdef_-" ) );
            Assert.That( ci.Values[1], Is.EqualTo( "My user|name!" ) );
            Assert.That( ci.ToString(), Is.EqualTo( "SignalR|0123456789abcdef_-|My%20user%7Cname%21" ) );
        }

        [Test]
        public void caller_id_fails_with_null_value()
        {
            string protocol = "SignalR";
            string[] values = new[]
            {
                "0123456789abcdef_-",
                null,
            };

            Assert.Throws<ArgumentNullException>(
                () => new CallerId( protocol, values )
                );
        }
    }
}
