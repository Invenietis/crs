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
        public Func<Type, string> CommandNameProvider { get; set; }

        public Func<Type, string> CommandDescriptionProvider { get; set; }

        public Func<Type, CKTrait> CommandTraitsProvider { get; set; }

        public string[] Assemblies { get; }

        public AutoRegisterOption( AssemblyName assemblyName )
        {
            Assemblies = new string[] { assemblyName.FullName };
        }


        public AutoRegisterOption( string[] assemblies )
        {
            Assemblies = assemblies;
            CommandNameProvider = t => t.Name;
            CommandDescriptionProvider = t => null;
            CommandTraitsProvider = t => null;
        }
    }
}
