using CK.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;
using CK.Crs.Runtime;
using System.Threading.Tasks;

namespace CK.Crs.Scalability.Tests
{
    public class CommandWorkerQueueTests
    {
        class FakeCrsHandler : ICrsHandler
        {
            public Task ProcessCommandAsync(CommandRequest command, CommandResponseBuilder response, IActivityMonitor monitor = null, CancellationToken cancellationToken = default(CancellationToken))
            {
                response.Set(new CommandErrorResponse( "Test", Guid.Empty));

                return Task.CompletedTask;
            }
        }

        [Fact]
        public void AddJobWhenQueueIsStartedShouldProcessThem()
        {
            ICrsHandler handler = new FakeCrsHandler();
            ICommandRequestFactory factory = new Internals.DefaultCommandRequestFactory();
            ICommandWorkerQueue queue = new Internals.InProcessWorkerQueue(handler, factory);
            try
            {
                var jobDone = 0;

                queue.JobCompleted += (object sender, JobCompletedEventArgs e) =>
                {
                    jobDone++;
                };

                queue.Add(CreateJob());
                queue.Add(CreateJob());
                queue.Add(CreateJob());
                queue.Add(CreateJob());

                Assert.Equal(0, jobDone);

                queue.Start();
                Thread.Sleep(1000);
                Assert.Equal(4, jobDone);

                queue.Add(CreateJob());
                Thread.Sleep(500);
                Assert.Equal(5, jobDone);
            }
            finally
            {
                queue.Dispose();
            }
        }


        private CommandJob CreateJob()
        {
            var monitor = new ActivityMonitor();
            var metaData = new CommandJobMetada(monitor, new CommandAction(Guid.NewGuid())
            {
                CallbackId = "c",
                Command = new object(),
                Description = new CommandDescription(typeof(object))
                {
                    HandlerType = typeof(object)
                }
            });
            var jsonPayload = Encoding.UTF8.GetBytes("#A2001RZZ;");
            return new CommandJob( metaData, jsonPayload);
        }
    }
}
