using Rebus.Config;
using System;
using CK.Crs;
using CK.Crs.Rebus;
using Rebus.Routing.TypeBased;
using CK.Core;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public delegate Action<Func<CommandModel, bool>> ConfigureQueue( string queueName );

    public static class CrsRebusExtensions
    {
        public static ICrsCoreBuilder AddRebusOneWay(
            this ICrsCoreBuilder builder,
            Action<RebusConfigurer> rebusConfigurer,
            params Action<ConfigureQueue>[] commandConfigs )
        {
            var activator = new GenericHandlerActivator(
                new Lazy<ICommandHandlerInvoker>(
                    () => builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandlerInvoker>() ),
                builder.Registry );

            var configurer = Configure.With( activator );
            configurer = configurer.Logging( l => l.Use( new GrandOutputRebusLoggerFactory() ) );

            configurer
                .Routing( l =>
                {
                    var typeBasedRouting = l.TypeBased();
                    foreach( var c in commandConfigs )
                    {
                        c( new ConfigureQueue( queueName => filter =>
                            {
                                foreach( var commandRegistration in builder.Registry.Registration.Where( filter ) )
                                {
                                    typeBasedRouting.Map( commandRegistration.CommandType, queueName );
                                }
                            }
                        ) );
                    }
                } );

            rebusConfigurer( configurer );

            var receiver = new RebusCommandReceiver( configurer, activator );
            builder.AddReceiver( s => receiver );

            return builder;
        }
    }
}
