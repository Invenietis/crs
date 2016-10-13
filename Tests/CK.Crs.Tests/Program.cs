using NUnitLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace CK.Crs.Tests
{
    public static class Program
    {
        public static int Main( string[] arguments )
        {
            //Debugger.Launch();
            int result;

            result = new AutoRun().Execute( arguments );
            return result;
        }
    }
}
