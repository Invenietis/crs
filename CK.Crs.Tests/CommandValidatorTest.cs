using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using NUnit.Framework;

namespace CK.Infrastructure.Commands.Tests
{
    [TestFixture]
    public class CommandValidatorTest
    {
        CancellationTokenSource _cancellationToken;
        public CommandValidatorTest()
        {
            _cancellationToken = new CancellationTokenSource();
        }

        class SomeCommand
        {
            [Required]
            [System.ComponentModel.DataAnnotations.Range( 10, 100 )]
            public int SomeIntegerValue { get; set; }

            [Required]
            [StringLength( 7 )]
            public string SomeField { get; set; }
        }

        [Test]
        [TestCase( "Success", 42, 0 )]
        [TestCase( null, 42, 1 )]
        [TestCase( "Value", 0, 1 )]
        [TestCase( null, 0, 2 )]
        [TestCase( "azeazeazeaze", 75, 1 )]
        [TestCase( "123456789", 100, 1 )]
        [TestCase( "123456789", 101, 2 )]
        public void DefaultCommandValidator( string fieldValue, int integerValue, int assert )
        {
            // Arrange
            var command = new SomeCommand
            {
                SomeField = fieldValue,
                SomeIntegerValue = integerValue
            };
            var descriptor = CreateCommandDescriptor<SomeCommand>();
            var commandContext = new CommandContext<SomeCommand>(
                new ActivityMonitor(),
                command,
                Guid.NewGuid(),
                descriptor.Descriptor.IsLongRunning,
                "3712",
                _cancellationToken.Token);

            var executionContext = new CommandExecutionContext( descriptor.Descriptor, commandContext );

            // Act
            DefaultCommandValidator v = new DefaultCommandValidator();
            v.OnCommandReceived( executionContext );

            // Assert
            if( assert == 0 )
            {
                Assert.That( executionContext.Response, Is.Null );
            }
            else
            {
                Assert.That( executionContext.Response, Is.Not.Null );
                Assert.That( executionContext.Response, Is.InstanceOf<CommandErrorResponse>() );
                string msg = executionContext.Response.Payload.ToString();
                Assert.That( assert, Is.EqualTo( msg.Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ).Length ) );
            }
        }

        private RoutedCommandDescriptor CreateCommandDescriptor<T>()
        {
            return new RoutedCommandDescriptor( new CommandRoutePath( "/prefix", typeof( T ).Name ), new CommandDescriptor
            {
                CommandType = typeof( T ),
                Decorators = CK.Core.Util.EmptyArray<Type>.Empty,
                IsLongRunning = false,
            } );
        }
    }
}
