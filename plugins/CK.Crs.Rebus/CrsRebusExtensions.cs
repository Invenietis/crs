using Rebus.Config;
using System;
using CK.Crs;
using CK.Crs.Rebus;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CrsRebusExtensions
    {

        public static ICrsCoreBuilder AddRebus( this ICrsCoreBuilder builder, Action<RebusConfigurer> rebusConfigurer )
        {
            var handlerFactory = new Lazy<IRequestHandlerFactory>( 
                () => builder.Services.BuildServiceProvider().GetRequiredService<IRequestHandlerFactory>() );

            var configurer = Configure.With( new GenericHandlerActivator( handlerFactory, builder.Registry ) );
            configurer = configurer.Logging( l => l.Use( new GrandOutputRebusLoggerFactory() ) );

            rebusConfigurer( configurer );

            var bus = configurer.Start();
            var resbusAdapter = new RebusAdapter( bus );
            builder.Services.AddSingleton<ICommandDispatcher>( resbusAdapter );
            builder.Services.AddSingleton<IEventPublisher>( resbusAdapter );

            return builder;
        }
    }
}
