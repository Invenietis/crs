using Rebus.Config;
using System;
using CK.Crs;
using CK.Crs.Rebus;
using Rebus.Routing.TypeBased;
using CK.Core;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsRebusExtensions
    {
        //public static ICrsCoreBuilder AddRebusSimpleSqlServer<TCommandAssembly, TEventAssembly>( this ICrsCoreBuilder builder, string conString )
        //{
        //    string crsDefaultQueueName = "crs-commands-queue";
        //    string crsDefaultSubscription = "crs-subscription-queue";

        //        //QueueName = "crs-commands-queue";

        //    return builder.AddRebus( c => c
        //               .Routing( r => r.TypeBased().MapAssemblyOf<TCommandAssembly>( crsDefaultQueueName ) )
        //               .Transport( t => t.UseSqlServer( conString, "tMessages", "commands" ) )
        //               .Subscriptions( s => s.StoreInSqlServer( conString, "tSubscriptions" ) ) )
        //}

        public class RebusOptions
        {
            public string CommandQueueName { get; set; }
        }

        public static ICrsCoreBuilder AddRebus( this ICrsCoreBuilder builder, Action<RebusConfigurer> rebusConfigurer )
        {
            var activator = new GenericHandlerActivator(
                new Lazy<ICommandHandlerInvoker>(
                    () => builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandlerInvoker>() ),
                builder.Registry );

            var configurer = Configure.With( activator );
            configurer = configurer.Logging( l => l.Use( new GrandOutputRebusLoggerFactory() ) );
            
            rebusConfigurer( configurer );
            
            builder.AddReceiver<RebusCommandReceiver>( s =>
            {
                var bus = configurer.Start();
                activator.SetBus( bus );
                return new RebusCommandReceiver( bus );
            } );
            
            return builder;
        }
    }
}
