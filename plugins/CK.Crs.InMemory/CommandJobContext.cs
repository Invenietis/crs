namespace CK.Crs.InMemory
{
    class CommandJobContext : CommandContext
    {
        public CommandJobContext( ICommandContext commandContext )
            : base( commandContext.CommandId, commandContext.Monitor, commandContext.Model, commandContext.CallerId )
        {
        }
    }
}
