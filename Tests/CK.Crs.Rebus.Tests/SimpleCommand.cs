using System;

namespace CK.Crs.Rebus.Tests.Messages
{
    public class SimpleCommand
    {
        public int ActorId { get; set; }
    }

    public class SimpleCommandWithResult
    {
        public int ActorId { get; set; }

        public class Result
        {
            public DateTime Date { get; set; }
        }
    }
    public interface ICommand<TResult> { }

    public class SimpleCommandWithDirectResult : ICommand<DateTime>
    {
        public int ActorId { get; set; }
    }
}
