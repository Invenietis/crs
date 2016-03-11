//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;
//using Assert = NUnit.Framework.Assert;
//using Is = NUnit.Framework.Is;
//using Microsoft.Extensions.DependencyInjection;
//using Moq;
//using CK.Crs.Runtime;

//namespace CK.Crs.Tests
//{
//    public abstract class Command
//    {
//        public int ActorId { get; set; }

//        public int AuthenticatedActorId { get; set; }
//    }

//    public class AmbientValueCheckerTest
//    {
//        class SimpleCommand : Command
//        {
//            public string Data { get; set; }
//        }

//        [Fact]
//        public async Task When_ActorId_And_AuthenticatedActorId_Are_Different_From_Ambient_Values_It_Should_Throw_SecurityException()
//        {
//            var sp = TestHelper.CreateServiceProvider( serviceCollection =>
//            {
//                serviceCollection
//                    .AddSingleton<ActorIdProvider>()
//                    .AddSingleton<IAuthenticationStore>( Mock.Of<IAuthenticationStore>());
//            } );

//            AmbientValuesFilter filter = new AmbientValuesFilter();
//            var description = new RoutedCommandDescriptor( new CommandRoutePath("/a", "simpleCommand"), new CommandDescription());
//            var commandModel = new SimpleCommand
//            {
//                ActorId = 12,
//                AuthenticatedActorId = 1
//            };
//            var command = new CommandExecutionContext(
//                ( ctx ) => TestHelper.MockEventPublisher(),
//                ( ctx ) => TestHelper.MockCommandScheduler(),
//                TestHelper.Monitor( Console.Out.WriteLine),
//                commandModel,
//                Guid.NewGuid(),
//                false,
//                String.Empty,
//                ClaimsPrincipal.Current,
//                default( CancellationToken));

//            var ambientValues = TestHelper.CreateAmbientValues( sp );

//            await filter.OnCommandReceived( new FilterContext( command.Monitor, description, ClaimsPrincipal.Current, command ) );

//        }
//    }
//}
