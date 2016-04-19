using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs.Runtime;
using Xunit;
using NUnit;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;
using Xunit.Abstractions;
using System.Security.Claims;

namespace CK.Crs.Tests
{
    public class CommandValidatorTest
    {
        ITestOutputHelper _output;
        CancellationTokenSource _cancellationToken;
        public CommandValidatorTest( ITestOutputHelper output )
        {
            _cancellationToken = new CancellationTokenSource();
            _output = output;
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
            var monitor =  TestHelper.Monitor( _output.WriteLine);
            // Arrange
            var command = new SomeCommand
            {
                SomeField = fieldValue,
                SomeIntegerValue = integerValue
            };
            var descriptor = CreateCommandDescriptor<SomeCommand>();

            var filterContext= new FilterContext( monitor, descriptor, ClaimsPrincipal.Current, command);
            // Act
            DefaultCommandValidator v = new DefaultCommandValidator();
            v.OnCommandReceived( filterContext );

            // Assert
            if( assert == 0 )
            {
                Assert.That( filterContext.Rejected, Is.False );
            }
            else
            {
                Assert.That( filterContext.Rejected, Is.True );
                Assert.That( filterContext.RejectReason, Is.Not.Null.And.Not.Empty );

                Assert.That( assert, Is.EqualTo( filterContext.RejectReason.Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ).Length ) );
            }
        }

        private CommandRoute CreateCommandDescriptor<T>()
        {
            return new CommandRoute( new CommandRoutePath( "/prefix", typeof( T ).Name ), new CommandDescription
            {
                CommandType = typeof( T ),
                Decorators = Util.Array.Empty<Type>(),
                IsLongRunning = false,
            } );
        }
    }
}
