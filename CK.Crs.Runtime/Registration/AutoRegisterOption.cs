using CK.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CK.Crs
{
    public class AutoRegisterOption
    {
        public Func<Type, bool> CommandHandlerFilter { get; set; }

        public Func<Type, string> CommandNameProvider { get; set; }

        public Func<Type, string> CommandDescriptionProvider { get; set; }

        public Func<Type, CKTrait> CommandTraitsProvider { get; set; }

        /// <summary>
        /// First argument is the command type. Second argument is the handler type. Returns a read only collection of <see cref="ICommandDecorator"/> types.
        /// </summary>
        public Func<Type, Type, IReadOnlyCollection<Type>> CommandDecorators { get; set; }

        public string[] Assemblies { get; }

        public AutoRegisterOption( AssemblyName assemblyName )
        {
            Assemblies = new string[] { assemblyName.FullName };
        }


        public AutoRegisterOption( string[] assemblies )
        {
            Assemblies = assemblies;
            CommandHandlerFilter = t => true;
            CommandNameProvider = CommandDescription.GetDefaultName;
            CommandDescriptionProvider = t => null;
            CommandTraitsProvider = t => null;
            CommandDecorators = CommandDescription.ExtractDecoratorsFromHandlerAttributes;
        }
    }
}
