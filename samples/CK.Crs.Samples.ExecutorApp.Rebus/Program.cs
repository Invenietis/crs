using CK.Crs.Samples.Messages;
using CK.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;
using Rebus.Routing.TypeBased;
using System;
using System.Threading;

namespace CK.Crs.Samples.ExecutorApp.Rebus
{
    class Program
    {
        static void Main( string[] args )
        {
            Console.WriteLine( "Hello World!" );

            ServiceCollection collection = new ServiceCollection();
            GrandOutput.EnsureActiveDefault( new GrandOutputConfiguration() );

            using( Startup s = new Startup() )
            {
                s.Configure( collection );
                s.ConfigureApplication( collection.BuildServiceProvider() );
            }
        }
    }

    class Startup : IDisposable
    {
        public void Configure( IServiceCollection services )
        {
            var conString = @"Server=.\SQLSERVER2016;Database=RebusQueue;Integrated Security=SSPI";

            services
                .AddCrsCore( c => c
                    .AddCommands( r => r.RegisterAssemblies( "CK.Crs.Samples.Handlers" ) ) )
                .AddRebus( c => c
                    .Transport( t => t.UseSqlServer( conString, "tMessages", "command_executor" ) )
                    .Routing( r => r.TypeBased().MapAssemblyOf<MessageBase>( "command_executor" ) ) );

        }

        public void ConfigureApplication( IServiceProvider services )
        {
        }

        ManualResetEvent _e = new ManualResetEvent( false );
        public void Dispose()
        {
            Console.CancelKeyPress += ( sender, e ) =>
            {
                _e.Set();
            };
            _e.WaitOne();
            _e.Dispose();
        }
    }
}