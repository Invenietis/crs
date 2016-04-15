using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using CK.Crs;
using CK.Crs.Tests.Fake;
using Microsoft.Extensions.DependencyInjection;
using CK.Core;
using CK.Crs.Tests.Handlers;
using CK.Crs.Runtime;
using Assert = NUnit.Framework.Assert;
using Is = NUnit.Framework.Is;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace CK.Crs.Tests
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class CommandReceiverTest
    {
        public CommandReceiverTest()
        {
            ApplicationServices = TestHelper.CreateServiceProvider( services =>
            {
                services.AddCommandReceiver( c =>
                {
                    c.Registry.Register<TransferAmountCommand, TransferAlwaysSuccessHandler>();
                } );
                services.AddSingleton<ICommandResponseDispatcher>( Mock.Of<ICommandResponseDispatcher>() );
            } );
        }

        public IServiceProvider ApplicationServices { get; private set; }

        [Test]
        public async Task CommandReceiver_Should_Be_Scoped_To_A_Route_Prefix()
        {
            var cmd =new Handlers.TransferAmountCommand
            {
                SourceAccountId = Guid.NewGuid(),
                DestinationAccountId = Guid.NewGuid(),
                Amount = 1000
            };

            var prefix = "/api";

            var routes = new CommandRouteCollection( prefix);
            {
                var receiver = ApplicationServices.GetRequiredService<ICommandReceiver>();
                {
                    var request = new CommandRequest( "/api", SerializeRequestBody( cmd ), ClaimsPrincipal.Current);
                    var response = await receiver.ProcessCommandAsync( request );
                    Assert.That( response, Is.Null );
                }
                routes.AddCommandRoute( new CommandDescription
                {
                    CommandType = typeof( TransferAmountCommand ),
                    HandlerType = typeof( TransferAlwaysSuccessHandler ),
                    IsLongRunning = false,
                    Name = typeof( TransferAmountCommand ).Name
                } );
                {
                    var request = new CommandRequest( "/api", SerializeRequestBody( cmd ), ClaimsPrincipal.Current);
                    var response = await receiver.ProcessCommandAsync( request );
                    Assert.That( response, Is.Null );
                }
                {
                    var request = new CommandRequest( "/api/TransferAmountCommand", SerializeRequestBody( cmd ), ClaimsPrincipal.Current);
                    var response = await receiver.ProcessCommandAsync( request );
                    Assert.That( response, Is.Not.Null );
                    Assert.That( response.CommandId, Is.Not.EqualTo( Guid.Empty ) );
                }
            }
        }


        private Stream SerializeRequestBody<T>( T command )
        {
            Stream s = new MemoryStream();
            StreamWriter sw = new StreamWriter(s);
            Newtonsoft.Json.JsonSerializer.Create().Serialize( sw, command );
            s.Seek( 0, SeekOrigin.Begin );
            return s;
        }
    }

}
