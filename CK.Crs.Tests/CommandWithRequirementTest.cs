using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CK.Authorization;
using CK.Core;
using CK.Crs.Extensions;
using CK.Crs.Runtime;
using Microsoft.AspNetCore.Authorization;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using NUnit.Framework;
using Moq;

namespace CK.Crs.Tests
{
    [Authorize]
    [MinGrantLevel( GrantLevel.Administrator )]
    public class CommandWithRequirementTest
    {
        public int ActorId { get; set; }

        [Required]
        [SecuredResource( "Document" )]
        public int DocumentId { get; set; }
    }

    public class SimpleHandler : CommandHandler<CommandWithRequirementTest>
    {
        protected override Task DoHandleAsync( ICommandExecutionContext context, CommandWithRequirementTest command )
        {
            return Task.FromResult<object>( null );
        }
    }

    public class MinGrantLevelAttribute : Attribute
    {
        public MinGrantLevelAttribute( GrantLevel minGrantLevel )
        {
        }
    }

    public class CommandPermissionTest
    {
        ITestOutputHelper _output;
        public CommandPermissionTest( ITestOutputHelper output )
        {
            _output = output;
        }
        [Xunit.Fact]
        public async Task Permission_Should_be_Challenged_By_AuthorizationFilter()
        {
            // Arrange
            var secureStore = new Mock<IProtectedResourceStore>();

            var serviceProvider = TestHelper.CreateServiceProvider( sp =>
            {
                sp.AddAuthorization();
                sp.AddSingleton<IAuthorizationHandler, MinGrantLevelHandler>();

                secureStore
                    .Setup( e => e.ChallengeResourceAsync( It.IsAny<ProtectedResourceChallenge>()) )
                    .Returns<ProtectedResourceChallenge>( e => Task.FromResult( new ProtectedResourceChallengeResult
                    {
                        Challenge = e,
                        ResultingLevel = GrantLevel.Administrator
                    }))
                    .Verifiable();
                sp.AddSingleton<IProtectedResourceStore>( secureStore.Object );
            });

            var monitor = TestHelper.Monitor(_output .WriteLine );
            var command = new CommandWithRequirementTest
            {
                ActorId = 12,
                DocumentId = 40
            };
            var description = new RoutedCommandDescriptor( new CommandRoutePath("/c/a"), new CommandDescriptor
            {
                CommandType = command.GetType(),
                HandlerType = typeof( SimpleHandler )
            });
            description.Descriptor.ExtraData.Add( "Permission", MinGrantLevel.Administrator );
            var ambientValues = TestHelper.CreateAmbientValues( serviceProvider );
            var filerContext = new FilterContext(  monitor, description, ClaimsPrincipal.Current, ambientValues, command);

            var authorizationService = serviceProvider.GetRequiredService<IAuthorizationService>();

            ClaimsPrincipal.ClaimsPrincipalSelector = () => new ClaimsPrincipal( new ClaimsIdentity( new[] { new Claim( ClaimTypes.Name, "John" ) }, "Test" ) );
            var authorizationFilter = new AuthorizationFilter( authorizationService );

            // Act
            await authorizationFilter.OnCommandReceived( filerContext );

            // Assert
            Assert.That( filerContext.IsRejected, Is.False );

            secureStore.VerifyAll();
        }
    }

    public static class SecuredResourceTypes
    {
        public static readonly string Document = "Document";
    }

    public class Document
    {
        public string ItemType
        {
            get { return SecuredResourceTypes.Document; }
        }

        public int DocumentId { get; set; }

        public int AclId { get; set; }

        public int ResourceId
        {
            get { return AclId; }
        }
    }
}
