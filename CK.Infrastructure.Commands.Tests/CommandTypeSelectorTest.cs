using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CK.Infrastructure.Commands.Tests
{
    [Collection( "CK.Infrastructure.Commands.Tests collection" )]
    public class CommandTypeSelectorTest
    {
        [Fact]
        public void should_find_a_type_by_name()
        {
            string commandTypeFullName = typeof(Stubs.TestCommand1).FullName;
            Assert.NotNull( Type.GetType( commandTypeFullName ) );

            string commandTypeName = typeof(Stubs.TestCommand1).FullName;
            Assert.NotNull( Type.GetType( commandTypeFullName ) );
        }
    }
}
