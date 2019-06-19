namespace CK.Crs.Samples.Messages
{
    public class SyncCommand : MessageBase, ICommand<SyncCommand.Result>
    {
        public class Result
        {
            public string Message { get; set; }
        }
    }
}
