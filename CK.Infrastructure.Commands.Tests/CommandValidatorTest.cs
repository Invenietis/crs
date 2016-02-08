﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using Microsoft.Dnx.Testing.Abstractions;
using Xunit;

namespace CK.Infrastructure.Commands.Tests
{
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
                Assert.Null( executionContext.Response );
            }
            else
            {
                Assert.NotNull( executionContext.Response );
                Assert.IsType<CommandErrorResponse>( executionContext.Response );
                string msg = executionContext.Response.Payload.ToString();
                Assert.Equal( assert, msg.Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ).Length );
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
