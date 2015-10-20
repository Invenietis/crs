using System;
using System.IO;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands.Tests
{
    internal class FakeCommandResultSerializer : ICommandResultSerializer
    {
        public Task SerializeCommandResult( ICommandResult result, Stream outputStream )
        {
            return Task.FromResult<object>( null );
        }
    }
}