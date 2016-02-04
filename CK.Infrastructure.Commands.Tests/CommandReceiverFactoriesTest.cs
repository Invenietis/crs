﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CK.Infrastructure.Commands.Tests
{
    public class CommandReceiverFactoriesTest
    {
        [Fact]
        public void When_Failed_Should_Return_Null()
        {
            // Arrange
            IServiceProvider sp = TestHelper.CreateServiceProvider( c =>
            {

            });
            // Act
            DefaultCommandReceiverFactories.DefaultCreateInstanceStrategy s = new DefaultCommandReceiverFactories.DefaultCreateInstanceStrategy( sp );
            ISomeType result = s.CreateInstanceOrDefault<ISomeType>( typeof( TypeWithConstructorWithParameters ) );

            // Assert
            Assert.Null( result );
        }

        [Fact]
        public void When_DefaultLambda_Should_InvokeIt()
        {
            // Arrange
            IServiceProvider sp = TestHelper.CreateServiceProvider( c =>
            {

            });
            // Act
            DefaultCommandReceiverFactories.DefaultCreateInstanceStrategy s = new DefaultCommandReceiverFactories.DefaultCreateInstanceStrategy( sp );
            ISomeType result = s.CreateInstanceOrDefault<ISomeType>( 
                typeof( TypeWithConstructorWithParameters ), 
                () => new TypeWithConstructorWithParameters( "WTF!!") );

            // Assert
            Assert.NotNull( result );
            Assert.Equal( "WTF!!", result.ShowMe() );
        }

        interface ISomeType
        {
            string ShowMe();
        }

        class TypeWithConstructorWithParameters : ISomeType
        {
            string _wtf;
            public TypeWithConstructorWithParameters( string wtf )
            {
                _wtf = wtf;
            }

            public string ShowMe()
            {
                return _wtf;
            }
        }
    }
}
