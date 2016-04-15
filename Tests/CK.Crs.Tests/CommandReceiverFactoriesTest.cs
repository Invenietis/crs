using System;
using NUnit.Framework;
using CK.Crs.Runtime;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;

namespace CK.Crs.Tests
{
    public class CommandReceiverTestoriesTest
    {
        [Test]
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
            Assert.That( result, Is.Not.Null );
        }

        [Test]
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
                () => new TypeWithConstructorWithParameters
                {
                    Message = "Hey"
                });

            // Assert
            Assert.That( result, Is.Not.Null );
            Assert.That( result.ShowMe(), Is.EqualTo( "Hey" ) );
        }

        interface ISomeType
        {
            string ShowMe();
        }

        class TypeWithConstructorWithParameters : ISomeType
        {
            public TypeWithConstructorWithParameters()
            {
            }

            public string Message { get; set; }
            public string ShowMe()
            {
                return Message;
            }
        }
    }
}
