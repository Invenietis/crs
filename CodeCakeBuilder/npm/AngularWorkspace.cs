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
                var outputPath = project.Value["architect"]?["build"]?["options"]?["outputPath"]?.Value<string>()
                    ?? projectPath;
                projects.Add(
                    NPMPublishedProject.Create(
                        globalInfo,
                        npmSolution,
                        path.Combine( projectPath ),
                        path.Combine( outputPath )
                    )
                );
            }
            return new AngularWorkspace( NPMPublishedProject.Create( globalInfo, npmSolution, path, path ), projects );
        }
    }
}
