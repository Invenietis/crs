using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CK.Core;
using CK.Crs;
using CK.Crs.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;

namespace CK.Crs.Tests
{
    public class ReflectionTest
    {
        [Test]
        public void auto_registration_simple()
        {
            IActivityMonitor monitor = new ActivityMonitor();
            monitor.Output.RegisterClient( new ActivityMonitorConsoleClient() );
            IServiceCollection services = Mock.Of<IServiceCollection>();
            ICommandRegistry registry = new CommandRegistry( services );
            AutoRegisterOption options = new AutoRegisterOption( "CK.Crs.Tests" );
            CommandRegistryExtensions.AutoRegisterSimple( registry, options, monitor );

            var r1 = registry.Registration.SingleOrDefault( x => x.CommandType == typeof( Command1 ));
            var r2 = registry.Registration.SingleOrDefault( x => x.CommandType == typeof( Command2 ));
            Assert.That( r1, Is.Not.Null );
            Assert.That( r2, Is.Not.Null );
            Assert.That( r1.HandlerType, Is.Not.Null.And.EqualTo( typeof( Handler ) ) );
            Assert.That( r2.HandlerType, Is.Not.Null.And.EqualTo( typeof( Handler ) ) );
            Assert.That( r1.Name, Is.Not.Null.And.EqualTo( "Command1" ) );
            Assert.That( r2.Name, Is.Not.Null.And.EqualTo( "Command2" ) );

            var r3 = registry.Registration.SingleOrDefault( x => x.CommandType == typeof( CreateUserCommand ));
            Assert.That( r3, Is.Not.Null );
            Assert.That( r3.HandlerType, Is.Not.Null.And.EqualTo( typeof( UserCommandHandler ) ) );
            Assert.That( r3.Name, Is.Not.Null.And.EqualTo( "CreateUser" ) );

        }
    }

    public class Command1
    {
    }
    public class Command2
    {
    }
    public class CreateUserCommand
    {
    }
    public class Handler : ICommandHandler<Command1>, ICommandHandler<Command2>
    {
        Task<object> ICommandHandler<Command1>.HandleAsync( ICommandExecutionContext commandContext, Command1 command )
        {
            throw new NotImplementedException();
        }

        Task<object> ICommandHandler.HandleAsync( ICommandExecutionContext commandContext, object command )
        {
            throw new NotImplementedException();
        }

        Task<object> ICommandHandler<Command2>.HandleAsync( ICommandExecutionContext commandContext, Command2 command )
        {
            throw new NotImplementedException();
        }
    }

    public class UserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        public Task<object> HandleAsync( ICommandExecutionContext commandContext, object command )
        {
            throw new NotImplementedException();
        }

        public Task<object> HandleAsync( ICommandExecutionContext commandContext, CreateUserCommand command )
        {
            throw new NotImplementedException();
        }
    }
}
