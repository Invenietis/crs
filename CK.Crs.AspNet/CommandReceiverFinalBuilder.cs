namespace CK.Crs.Runtime
{
    public sealed class CommandReceiverFinalBuilder : CommandReceiverBuilder
    {
        protected override void ApplyDefaultConfiguration( CommandReceiverConfiguration config )
        {
            base.ApplyDefaultConfiguration( config );
            config.Pipeline.UseDefault().UseSyncCommandExecutor().UseJsonResponseWriter();
        }
    }
}
