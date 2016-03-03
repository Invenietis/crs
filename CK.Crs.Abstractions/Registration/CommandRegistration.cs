using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    public class CommandRegistration : ICommandRegistrationWithFilter, ICommandRegistration
    {
        RoutedCommandDescriptor _routedCommandDescriptor;
        CommandDescriptor _commandDescription;

        public CommandRegistration( RoutedCommandDescriptor description )
        {
            _routedCommandDescriptor = description;
            _commandDescription = description.Descriptor;
        }

        public CommandRegistration( CommandDescriptor description )
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

        CommandRegistration AddPermission( object permission )
        {
            _commandDescription.Permissions = _commandDescription.Permissions.Union( new[] { permission } ).ToArray();
            return this;
        }

        CommandRegistration CommandName( string commandName )
        {
            _commandDescription.Name = commandName;
            return this;
        }

        CommandRegistration IsLongRunning()
        {
            _commandDescription.IsLongRunning = true;
            return this;
        }

        CommandRegistration AddFilter<TFilter>() where TFilter : ICommandFilter
        {
            _routedCommandDescriptor.Filters = _routedCommandDescriptor.Filters.Union( new[] { typeof( TFilter ) } ).ToArray();
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

        ICommandConfiguration<ICommandRegistrationWithFilter> ICommandConfiguration<ICommandRegistrationWithFilter>.IsLongRunning()
        {
            return IsLongRunning();
        }
        ICommandConfiguration<ICommandRegistrationWithFilter> ICommandConfiguration<ICommandRegistrationWithFilter>.AddPermission( object permission )
        {
            return AddPermission( permission );
        }

        ICommandConfigurationWithFilter<ICommandRegistrationWithFilter> ICommandConfigurationWithFilter<ICommandRegistrationWithFilter>.AddFilter<T1>()
        {
            return AddFilter<T1>();
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.AddDecorator<T1>()
        {
            return AddDecorator<T1>();
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.AddPermission( object permission )
        {
            return AddPermission( permission );
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.CommandName( string commandName )
        {
            return CommandName( commandName );
        }

        ICommandConfiguration<ICommandRegistration> ICommandConfiguration<ICommandRegistration>.IsLongRunning()
        {
            return IsLongRunning();
        }

        ICommandConfigurationWithHandling<ICommandRegistration> ICommandConfigurationWithHandling<ICommandRegistration>.HandledBy<THandler>()
        {
            return HandledBy<THandler>();
        }
    }

}
