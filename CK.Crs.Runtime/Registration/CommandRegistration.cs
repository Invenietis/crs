using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class CommandRegistration : ICommandRegistrationWithFilter, ICommandRegistration
    {
        CommandRoute _CommandRoute;
        CommandDescription _commandDescription;

        public CommandRegistration( CommandRoute description )
        {
            _CommandRoute = description;
            _commandDescription = description.Descriptor;
        }

        public CommandRegistration( CommandDescription description )
        {
            _commandDescription = description;
        }

        CommandRegistration HandledBy<THandler>() where THandler : ICommandHandler
        {
            _commandDescription.HandlerType = typeof( THandler );
            return this;
        }

        CommandRegistration AddDecorator<TDecorator>() where TDecorator : ICommandDecorator
        {
            _commandDescription.Decorators = _commandDescription.Decorators.Union( new[] { typeof( TDecorator ) } ).ToArray();
            return this;
        }

        CommandRegistration AddExtraData( string key, object value )
        {
            _commandDescription.ExtraData.Add( key, value );
            return this;
        }

        CommandRegistration CommandName( string commandName )
        {
            _commandDescription.Name = commandName;
            return this;
        }

        CommandRegistration AddFilter<TFilter>() where TFilter : ICommandFilter
        {
            _CommandRoute.Filters = _CommandRoute.Filters.Union( new[] { typeof( TFilter ) } ).ToArray();
            return this;
        }

        ICommandConfiguration<ICommandRegistrationWithFilter> ICommandConfiguration<ICommandRegistrationWithFilter>.AddDecorator<T1>()
        {
            return AddDecorator<T1>();
        }

        ICommandConfiguration<ICommandRegistrationWithFilter> ICommandConfiguration<ICommandRegistrationWithFilter>.CommandName( string commandName )
        {
            return CommandName( commandName );
        }

        ICommandConfiguration<ICommandRegistrationWithFilter> ICommandConfiguration<ICommandRegistrationWithFilter>.AddExtraData( string key, object value )
        {
            return AddExtraData( key, value );
        }

        ICommandConfigurationWithFilter<ICommandRegistrationWithFilter> ICommandConfigurationWithFilter<ICommandRegistrationWithFilter>.AddFilter<T1>()
        {
            return AddFilter<T1>();
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.AddDecorator<T1>()
        {
            return AddDecorator<T1>();
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.AddExtraData( string key, object value )
        {
            return AddExtraData( key, value );
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.CommandName( string commandName )
        {
            return CommandName( commandName );
        }

        ICommandConfigurationWithHandling<ICommandRegistration> ICommandConfigurationWithHandling<ICommandRegistration>.HandledBy<THandler>()
        {
            return HandledBy<THandler>();
        }
    }

}
