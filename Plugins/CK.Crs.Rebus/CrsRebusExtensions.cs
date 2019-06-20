using Rebus.Config;
using System;
using CK.Crs;
using CK.Crs.Rebus;
using Rebus.Routing.TypeBased;
using System.Linq;
using CK.Crs.Results;

namespace Microsoft.Extensions.DependencyInjection
{
    public delegate Action<Func<ICommandModel, bool>> ConfigureQueue( string queueName );

    public static class CrsRebusExtensions
    {
        public static ICrsCoreBuilder AddRebus( this ICrsCoreBuilder builder, Action<RebusConfigurer> rebusConfigurer, params Action<ConfigureQueue>[] commandConfigs )
        {
            builder.Services.AddSingleton<RebusCommandReceiver>( services =>
            {
                var activator = new CrsHandlerActivator(
                    services.GetRequiredService<ICommandHandlerInvoker>(),
                    services.GetRequiredService<IResultReceiverProvider>(),
                    builder.Registry );

                var configurer = Configure
                    .With( activator )
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
                var bus = configurer.Start();
                activator.SetBus( bus );
                return new RebusCommandReceiver( bus );
            } );
            builder.AddReceiver<RebusCommandReceiver>();

            return builder;
        }
    }
}
