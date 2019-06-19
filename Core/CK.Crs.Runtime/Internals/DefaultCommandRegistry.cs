using System;
using System.Collections.Generic;
using CK.Core;

namespace CK.Crs
{
    class DefaultCommandRegistry : ICommandRegistry
    {
        List<ICommandModel> _commands = new List<ICommandModel>();

        public DefaultCommandRegistry( CKTraitContext traitContext )
        {
            TraitContext = traitContext;
        }
        public CKTraitContext TraitContext { get; }

        public IEnumerable<ICommandModel> Registration => _commands;

        public void Register( ICommandModel descriptor ) => _commands.Add( descriptor );

        public ICommandRegistration Register( Type commandType, Type resultType = null )
        {
            var model = new CommandModel( commandType, resultType, TraitContext );
            return AddRegistration( model );
        }

        private ICommandRegistration AddRegistration( CommandModel model )
        {
            var registration = new CommandRegistration( this, model );
            Register( model );
            if( model.ResultType != null )
            {
                registration.SetResultTag();
            }
            return registration;
        }

        public ICommandModel GetCommandByName( CommandName name )
        {
            return _commands.Find( n => n.Name == name );
        }
    }
}
