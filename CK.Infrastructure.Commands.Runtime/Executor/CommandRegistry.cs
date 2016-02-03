using System;
using System.Collections.Generic;
using CK.Core;
using System.Linq;

namespace CK.Infrastructure.Commands
{
    public class CommandRegistry : ICommandRegistry
    {
        List<CommandDescriptor> Map { get; } = new List<CommandDescriptor>();

        public IEnumerable<CommandDescriptor> Registration
        {
            get { return Map; }
        }

        public CommandDescriptor Register( Type commandType, Type handlerType, string commandName, bool isLongRunning, params Type[] decorators )
        {
            if( String.IsNullOrEmpty( commandName ) )
                commandName = GetCommandNameFromType( commandType );

            var desc = new CommandDescriptor
            {
                CommandType = commandType,
                HandlerType = handlerType,
                Name = commandName,
                IsLongRunning = isLongRunning
            };

            desc.Decorators = ExtractDecoratorsFromHandlerAttributes( desc.CommandType, desc.HandlerType )
                 .Union( ApplyGlobalDecorators(), EqualityComparer<Type>.Default )
                 .ToArray();

            Map.Add( desc );

            return desc;
        }

        static string GetCommandNameFromType( Type commandType )
        {
            return commandType.Name;
        }

        private IEnumerable<Type> ApplyGlobalDecorators()
        {
            //if( _globalDecorators.IsValueCreated )
            //{
            //    return _globalDecorators.Value;
            //}
            return Enumerable.Empty<Type>();
        }

        private static IReadOnlyCollection<Type> ExtractDecoratorsFromHandlerAttributes( Type commandType, Type handlerType )
        {
            return handlerType.GetCustomAttributes( true ).OfType<ICommandDecorator>().Select( a => a.GetType() ).ToArray();
        }
    }
}