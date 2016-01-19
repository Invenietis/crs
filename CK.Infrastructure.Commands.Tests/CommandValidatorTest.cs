using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Dnx.Testing.Abstractions;
using Xunit;

namespace CK.Infrastructure.Commands.Tests
{
    public class CommandValidatorTest
    {

        class SomeCommand
        {
            [Required]
            [Range( 10, 100 )]
            public int SomeIntegerValue { get; set; }

            [Required]
            [StringLength( 7 )]
            public string SomeField { get; set; }
        }

        [Theory]
        [InlineData( "Success", 42, 0 )]
        [InlineData( null, 42, 1 )]
        [InlineData( "Value", 0, 1 )]
        [InlineData( null, 0, 2 )]
        [InlineData( "azeazeazeaze", 75, 1 )]
        [InlineData( "123456789", 100, 1 )]
        [InlineData( "123456789", 101, 2 )]
        public void DefaultCommandValidator( string fieldValue, int integerValue, int assert )
        {
            // Arrange
            var command = new SomeCommand
            {
                SomeField = fieldValue,
                SomeIntegerValue = integerValue
            };
            var descriptor = CreateCommandDescriptor<SomeCommand>();
            var commandContext = new CommandContext<SomeCommand>( new ActivityMonitor(), command, Guid.NewGuid(), descriptor.IsLongRunning, "3712" );
            var executionContext = new CommandExecutionContext( descriptor, commandContext );

            // Act
            DefaultCommandValidator v = new DefaultCommandValidator();
            ICollection<ValidationResult> results;
            bool valid = v.TryValidate( executionContext, out results );

            // Assert
            if( assert == 0 )
            {
                Assert.True( valid );
                Assert.Empty( results );
            }
            else
            {
                Assert.False( valid );
                Assert.NotEmpty( results );
                Assert.Equal( assert, results.Count );
            }
        }

        private CommandDescriptor CreateCommandDescriptor<T>()
        {
            return new CommandDescriptor
            {
                CommandType = typeof( T ),
                Decorators = CK.Core.Util.EmptyArray<Type>.Empty,
                IsLongRunning = false,
                Route = new CommandRoutePath( "/prefix", typeof( T ).Name )
            };
        }
    }
}
