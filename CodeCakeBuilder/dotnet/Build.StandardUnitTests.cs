using Cake.Common.Diagnostics;
using Cake.Common.IO;
using Cake.Common.Solution;
using Cake.Common.Tools.DotNetCore;
using Cake.Common.Tools.DotNetCore.Test;
using Cake.Common.Tools.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace CodeCake
{
    public partial class Build
    {

        void StandardUnitTests( StandardGlobalInfo globalInfo, IEnumerable<SolutionProject> testProjects )
        {
            string memoryFilePath = $"CodeCakeBuilder/UnitTestsDone.{globalInfo.GitInfo.CommitSha}.txt";

            void WriteTestDone( Cake.Core.IO.FilePath test )
            {
                System.IO.File.AppendAllLines( memoryFilePath, new[] { test.ToString() } );
            }

            bool CheckTestDone( Cake.Core.IO.FilePath test )
            {
                bool done = System.IO.File.Exists( memoryFilePath )
                            ? System.IO.File.ReadAllLines( memoryFilePath ).Contains( test.ToString() )
                            : false;
                if( done ) Cake.Information( "Test already done on this commit." );
                return done;
            }

            var testDlls = testProjects.Select( p =>
                         new
                         {
                             CSProjPath = p.Path,
                             ProjectPath = p.Path.GetDirectory(),
                             NetCoreAppDll21 = p.Path.GetDirectory().CombineWithFilePath( "bin/" + globalInfo.BuildConfiguration + "/netcoreapp2.1/" + p.Name + ".dll" ),
                             NetCoreAppDll22 = p.Path.GetDirectory().CombineWithFilePath( "bin/" + globalInfo.BuildConfiguration + "/netcoreapp2.2/" + p.Name + ".dll" ),
                             Net461Dll = p.Path.GetDirectory().CombineWithFilePath( "bin/" + globalInfo.BuildConfiguration + "/net461/" + p.Name + ".dll" ),
                             Net461Exe = p.Path.GetDirectory().CombineWithFilePath( "bin/" + globalInfo.BuildConfiguration + "/net461/" + p.Name + ".exe" ),
                         } );

            foreach( var test in testDlls )
            {
                var net461 = Cake.FileExists( test.Net461Dll )
                                ? test.Net461Dll
                                : Cake.FileExists( test.Net461Exe )
                                    ? test.Net461Exe
                                    : null;
                if( net461 != null )
                {
                    Cake.Information( $"Testing via NUnit (net461): {net461}" );
                    if( !CheckTestDone( net461 ) )
                    {
                        Cake.NUnit( new[] { net461 }, new NUnitSettings()
                        {
                            Framework = "v4.5",
                            ResultsFile = test.ProjectPath.CombineWithFilePath( "TestResult.Net461.xml" )
                        } );
                        WriteTestDone( net461 );
                    }
                }
                if( Cake.FileExists( test.NetCoreAppDll21 ) )
                {
                    TestNetCore( test.CSProjPath.FullPath, test.NetCoreAppDll21, "netcoreapp2.1" );
                }
                if( Cake.FileExists( test.NetCoreAppDll22 ) )
                {
                    TestNetCore( test.CSProjPath.FullPath, test.NetCoreAppDll22, "netcoreapp2.2" );
                }
            }

            void TestNetCore( string projectPath, Cake.Core.IO.FilePath dllFilePath, string framework )
            {
                var e = XDocument.Load( projectPath ).Root;
                if( e.Descendants( "PackageReference" ).Any( r => r.Attribute( "Include" )?.Value == "Microsoft.NET.Test.Sdk" ) )
                {
                    Cake.Information( $"Testing via VSTest ({framework}): {dllFilePath}" );
                    if( CheckTestDone( dllFilePath ) ) return;
                    Cake.DotNetCoreTest( projectPath, new DotNetCoreTestSettings()
                    {
                        Configuration = globalInfo.BuildConfiguration,
                        Framework = framework,
                        NoRestore = true,
                        NoBuild = true,
                        Logger = "trx"
                    } );
                }
                else
                {
                    Cake.Information( $"Testing via NUnitLite ({framework}): {dllFilePath}" );
                    if( CheckTestDone( dllFilePath ) ) return;
                    Cake.DotNetCoreExecute( dllFilePath );
                }
                WriteTestDone( dllFilePath );
            }
        }

    }
}
