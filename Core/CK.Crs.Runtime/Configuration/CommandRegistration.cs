using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs
{
    /// <summary>
    /// Implementation of the fluent registration API.
    /// </summary>
    class CommandRegistration : ICommandRegistration
    {
        readonly CommandModel _model;
        readonly ICommandRegistry _registry;

        public ICommandModel Model => _model;

        /// <summary>
        /// Creates a <see cref="CommandRegistration"/> from a <see cref="CommandModel"/>
        /// </summary>
        /// <param name="model"></param>
        public CommandRegistration( ICommandRegistry registry, CommandModel model )
        {
            _registry = registry;
            _model = model;
        }

        CommandRegistration Handler( Type handler )
        {
            _model.HandlerType = handler;

            return this;
        }

        CommandRegistration CommandName( string commandName )
        {
            _model.Name = commandName;
            return this;
        }

        CommandRegistration CustomBinder( ICommandBinder commandBinder )
        {
            _model.Binder = commandBinder;
            return this;
        }

        CommandRegistration AddFilters( IEnumerable<Type> filters )
        {
            _model.Filters = filters;
            return this;
        }

        ICommandRegistration ICommandRegistration.CommandName( string commandName )
        {
            return CommandName( commandName );
        }

        ICommandRegistration ICommandRegistration.CustomBinder( ICommandBinder commandBinder)
        {
            return CustomBinder( commandBinder );
        }

        ICommandRegistration ICommandRegistration.AddFilters( IEnumerable<Type> filters )
        {
            return AddFilters( filters );
        }

        ICommandRegistration ICommandRegistration.Handler( Type handler )
        {
            return Handler( handler );
        }
    }
}
