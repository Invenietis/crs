using System;
using NUnit.Framework;
using CK.Crs.Runtime;
using Xunit;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;

namespace CK.Crs.Tests
{
    public class CommandReceiverFactoriesTest
    {
        [Test][Fact]
        public void When_Failed_Should_Return_Null()
        {
            // Arrange
            IServiceProvider sp = TestHelper.CreateServiceProvider( c =>
            {

            });
            // Act
            DefaultFactories.DefaultCreateInstanceStrategy s = new DefaultFactories.DefaultCreateInstanceStrategy( sp );
            ISomeType result = s.CreateInstanceOrDefault<ISomeType>( typeof( TypeWithConstructorWithParameters ) );

            // Assert
            Assert.Null( result );
        }

        [Test][Fact]
        public void When_DefaultLambda_Should_InvokeIt()
        {
            // Arrange
            IServiceProvider sp = TestHelper.CreateServiceProvider( c =>
            {

            });
            // Act
            DefaultFactories.DefaultCreateInstanceStrategy s = new DefaultFactories.DefaultCreateInstanceStrategy( sp );
            ISomeType result = s.CreateInstanceOrDefault<ISomeType>( 
                typeof( TypeWithConstructorWithParameters ), 
                () => new TypeWithConstructorWithParameters( "WTF!!") );

            // Assert
            Assert.NotNull( result );
            Assert.AreEqual( "WTF!!", result.ShowMe() );
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
