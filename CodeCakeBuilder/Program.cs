using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Code.Cake;
using CodeCake;
using Microsoft.Extensions.PlatformAbstractions;

namespace CodeCakeBuilder
{
    public class Program
    {
        public static int Main( string[] args )
        {
            string solutionDir = Path.GetDirectoryName( PlatformServices.Default.Application.ApplicationBasePath );
            var app = new CodeCakeApplication( solutionDir, typeof(Program).Assembly );
            bool interactive = !args.Contains( '-' + InteractiveAliases.NoInteractionArgument, StringComparer.OrdinalIgnoreCase );
            int result = app.Run( args );
            Console.WriteLine();
            if( interactive )
            {
                Console.WriteLine( "Hit any key to exit. (Use -{0} parameter to exit immediately)", InteractiveAliases.NoInteractionArgument );
                Console.ReadKey();
            }
            return result;
        }
    }
}
