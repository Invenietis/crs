using Cake.Common.Diagnostics;
using CK.Text;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeCake
{
    public class AngularWorkspace : NPMProjectContainer
    {
        public NPMProject WorkspaceProject { get; }

        AngularWorkspace(
            NPMProject workspaceProject,
            IReadOnlyList<NPMProject> projects )
            : base()
        {
            WorkspaceProject = workspaceProject;
            foreach( var p in projects )
            {
                Add( p );
            }
        }
        public static AngularWorkspace Create( StandardGlobalInfo globalInfo, NPMSolution npmSolution, NormalizedPath path )
        {
            NormalizedPath packageJsonPath = path.AppendPart( "package.json" );
            NormalizedPath angularJsonPath = path.AppendPart( "angular.json" );

            JObject packageJson = JObject.Parse( File.ReadAllText( packageJsonPath ) );
            JObject angularJson = JObject.Parse( File.ReadAllText( angularJsonPath ) );
            if( !(packageJson["private"]?.ToObject<bool>() ?? false) ) throw new InvalidDataException( "A workspace project should be private." );
            List<NPMProject> projects = new List<NPMProject>();
            var jsonProject = angularJson["projects"].ToObject<JObject>();
            foreach( var project in jsonProject.Properties() )
            {
                var projectPath = project.Value["root"].ToString();
                var options = project.Value["architect"]["build"]["options"];
                string outputPathJson = options["outputPath"]?.Value<string>();
                bool havePath = outputPathJson != null;
                string ngPackagePath = options["project"]?.Value<string>();
                bool haveNgPackageJson = ngPackagePath != null;
                if( havePath && haveNgPackageJson ) throw new NotImplementedException();//I don't know what to do in this case.
                NormalizedPath outputPath;
                NormalizedPath ngPackagePathFullPath = path.Combine( ngPackagePath );
                if( haveNgPackageJson )
                {
                    JObject ngPackage = JObject.Parse( File.ReadAllText( ngPackagePathFullPath ) );
                    string dest = ngPackage["dest"]?.Value<string>();
                    if( dest == null ) throw new InvalidDataException( "ng package does not contain dest path." );
                    outputPath = ngPackagePathFullPath.RemoveLastPart().Combine( dest ).ResolveDots();
                }
                else if( havePath )
                {
                    outputPath = path.Combine( outputPathJson );
                }
                else
                {
                    globalInfo.Cake.Warning( $"No path found for angular project '{path}'." );
                    outputPath = path.Combine( projectPath );
                }

                projects.Add(
                    NPMPublishedProject.Create(
                        globalInfo,
                        npmSolution,
                        path.Combine( projectPath ),
                        outputPath
                    )
                );
            }
            return new AngularWorkspace( NPMPublishedProject.Create( globalInfo, npmSolution, path, path ), projects );
        }
    }
}
