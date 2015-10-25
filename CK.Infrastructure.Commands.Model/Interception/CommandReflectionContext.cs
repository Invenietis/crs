using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CK.Infrastructure.Commands
{
    public class CommandReflectionContext
    {
        public CommandReflectionContext( object command, Type handlerType )
        {
            Command = command;
            CommandType = command.GetType().GetTypeInfo();
            HandlerType = handlerType.GetTypeInfo();

            var q = (from m in handlerType.GetTypeInfo().DeclaredMethods
                     let parameters = m.GetParameters()
                     where
                         (m.Name == "DoHandleAsync" && CommandType.IsAssignableFrom( parameters[0].ParameterType.GetTypeInfo() ) ) ||
                         (m.Name == "HandleAsync")
                     select new
                     {
                         HandleMethod = m,
#if !DNXCORE50 && !DOTNET
                         Attributes = HandlerType.GetCustomAttributes( true ).OfType<HandlerAttributeBase>()
#else
                         Attributes = new HandlerAttributeBase[0]
#endif                        
                     }).FirstOrDefault();

            MethodInfo = q.HandleMethod;
            Decorators = q.Attributes.ToArray();
        }

        /// <summary>
        /// Gets the request associated to this execution context.
        /// </summary>
        public object Command { get; private set; }

        /// <summary>
        /// Gets the <see cref="MethodInfo"/> of the current handler
        /// </summary>
        public MethodInfo MethodInfo { get; internal set; }

        /// <summary>
        /// Gets all <see cref="HandlerAttributeBase"/> on the current handler metho
        /// </summary>
        public IReadOnlyCollection<IDecorator> Decorators { get; private set; }

        /// <summary>
        /// Gets the hanler <see cref="Type"/>
        /// </summary>
        public TypeInfo HandlerType { get; set; }

        /// <summary>
        /// Gets the command <see cref="Type"/>
        /// </summary>
        public TypeInfo CommandType { get; set; }
    }
}