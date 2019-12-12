using CK.Text;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeCake
{
    public class AngularWorkspace : NPMProjectContainer
    {
        public NPMProject WorkspaceProject { get; }
        public NormalizedPath OutputPath { get; }

        AngularWorkspace(
            NPMProject workspaceProject,
            IReadOnlyList<NPMProject> projects,
            NormalizedPath outputPath )
            : base()
        {
            WorkspaceProject = workspaceProject;
            foreach( var p in projects )
            {
                Add( p );
            }
            OutputPath = outputPath;
        }
        public static AngularWorkspace Create( StandardGlobalInfo globalInfo, NPMSolution npmSolution, NormalizedPath path, NormalizedPath outputFolder )
        {
            NormalizedPath packageJsonPath = path.AppendPart( "package.json" );
            NormalizedPath angularJsonPath = path.AppendPart( "angular.json" );

            JObject packageJson = JObject.Parse( File.ReadAllText( packageJsonPath ) );
            JObject angularJson = JObject.Parse( File.ReadAllText( angularJsonPath ) );
            if( !(packageJson["private"]?.ToObject<bool>() ?? false) ) throw new InvalidDataException( "A workspace project should be private." );
            string solutionName = packageJson["name"].ToString();
            List<string> unscopedNames = angularJson["projects"].ToObject<JObject>().Properties().Select( p => p.Name ).ToList();
            List<NPMProject> projects = unscopedNames.Select(
                p => NPMPublishedProject.Create(
                    globalInfo,
                    npmSolution,
                    path.Combine( new NormalizedPath( angularJson["projects"][p]["root"].ToString() ) ),
                    outputFolder.AppendPart( p )
                )
            ).ToList();
            return new AngularWorkspace( projects.Single( p => p.DirectoryPath == path ), projects, outputFolder );
        }
    }
}
