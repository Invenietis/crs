using CK.Crs.CommandDiscoverer.Attributes;

namespace CK.Crs.Samples.AspNetCoreBasicApp.Commands
{
    [CommandFireAndForget]
    [CommandName( "MyDeferredCommand" )]
    public class MyDeferredCommand : ICommand<string>
    {
    }
}