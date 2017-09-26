using Rebus.Config;
using System;
using CK.Crs;
using CK.Crs.Rebus;
using Rebus.Routing.TypeBased;
using System.Linq;
using CK.Monitoring;

namespace Microsoft.Extensions.DependencyInjection
{
    public delegate Action<Func<CommandModel, bool>> ConfigureQueue( string queueName );

    public static class CrsRebusExtensions
    {
        public static ICrsCoreBuilder AddRebus(
            this ICrsCoreBuilder builder,
            Action<RebusConfigurer> rebusConfigurer,
            params Action<ConfigureQueue>[] commandConfigs )
        {
            // TODO... find a better way than Lazy and SP building
            var activator = new GenericHandlerActivator(
                new Lazy<ICommandHandlerInvoker>(
                    () => builder.Services.BuildServiceProvider().GetRequiredService<ICommandHandlerInvoker>() ),
                new Lazy<IResultReceiverProvider>(
                    () => builder.Services.BuildServiceProvider().GetRequiredService<IResultReceiverProvider>() ),
                builder.Registry );

            var configurer = Configure.With( activator );
            configurer = configurer.Logging( l => l.Use( new GrandOutputRebusLoggerFactory( GrandOutput.Default ) ) );

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
