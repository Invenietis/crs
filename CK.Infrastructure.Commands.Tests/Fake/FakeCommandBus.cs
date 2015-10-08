using System;
using System.Threading.Tasks;

namespace CK.Infrastructure.Commands.Tests
{
    internal class FakeCommandBus : ICommandBus
    {
        public Task<ICommandResult> SendAsync( object command )
        {
            throw new NotImplementedException();
        }
    }
}