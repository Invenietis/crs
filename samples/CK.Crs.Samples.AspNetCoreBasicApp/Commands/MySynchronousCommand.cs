using CK.Crs.CommandDiscoverer.Attributes;

namespace CK.Crs.Samples.AspNetCoreBasicApp.Commands
{
    [CommandName( "MySynchronousCommand" )]
    public class MySynchronousCommand : ICommand<string>
    {
    }
}