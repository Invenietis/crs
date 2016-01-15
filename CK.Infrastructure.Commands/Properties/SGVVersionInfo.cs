
// Automatically generated by 'sgv prebuild' 1.0.0-beta.
using System;
using System.Reflection;

[assembly: AssemblyVersion(@"1.0")]
[assembly: AssemblyFileVersion(@"0.18626.44848.6178")]
[assembly: AssemblyInformationalVersion(@"1.0.0-b Sha:8af9468d728be0984c35223b910041d82bade785 User:CEDRIC-LAPTOP\cedri_000")]

[assembly: SimpleGitVersionInfo( "1.0.0-beta", "2016-01-13 09:47:32Z", "0.12.1" )]

/// <summary>
/// Automatically generated by 'sgv prebuild' 1.0.0-beta. 
/// </summary>
class SimpleGitVersionInfoAttribute : Attribute
{
    public SimpleGitVersionInfoAttribute( string semVer, string buildTimeUtc, string sgvVersion )
    {
        SemVer = semVer;
        BuildTimeUtc = buildTimeUtc;
        SGVVersion = sgvVersion;
    }

    public readonly string SemVer;
    public readonly string BuildTimeUtc;
    public readonly string SGVVersion;
    
    public override string ToString()
    {
        return String.Format( "SemVer: {0}, BuildTimeUtc: {1}, SGVVersion: {2}", SemVer, BuildTimeUtc, SGVVersion );
    }
}
