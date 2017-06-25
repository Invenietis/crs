using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CK.Core;

namespace CK.Crs.Scalability.Internals
{
    /// <summary>
    /// Represents a in memory worker process with no state management, only an internal job queue.
    /// It is not thread safe. If its managed to use concurrently with other queues, please consider using a
    /// new thread-safe <see cref="ICommandWorkerQueue"/> implementation which uses this one as a thread-affine worker.
    /// </summary>
    class InProcessWorkerQueue : ICommandWorkerQueue, IDisposable
    {
        readonly BlockingCollection<CommandJob> _queue;
        readonly CancellationTokenSource _source;

        public event EventHandler<JobCompletedEventArgs> JobCompleted;

        readonly ICrsHandler _handler;
        readonly ICommandRequestFactory _factory;
        public InProcessWorkerQueue(ICrsHandler handler, ICommandRequestFactory factory )
        {
            _queue = new BlockingCollection<CommandJob>();
            _source = new CancellationTokenSource();

            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public void Add(CommandJob command)
        {
            _queue.Add(command);
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (!_queue.IsCompleted)
                {
                    var job = _queue.Take(_source.Token);
                    using (job.Monitor.OpenInfo().Send("Getting job {0} for execution.", job.MetaData.CommandType))
                    {
                        var commandRequest = _factory.CreateFrom(job);
                        var responseBuilder = new Runtime.CommandResponseBuilder();

                        try
                        {
                            await _handler
                                .ProcessCommandAsync(commandRequest, responseBuilder, job.Monitor, _source.Token)
                                .ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            job.Monitor.Error().Send(ex);
                            if (!responseBuilder.HasReponse)
                            {
                                responseBuilder.Set(new Runtime.CommandErrorResponse(ex, Guid.Empty));
                            }
                        }

                        try
                        {
                            JobCompleted?.Invoke(this, new JobCompletedEventArgs(job, responseBuilder));
                        }
                        catch (Exception ex)
                        {
                            job.Monitor.Error().Send(ex);
                        }
                    }
                }
            });
        }

        public void Dispose()
        {
            _queue?.CompleteAdding();
            _source?.Cancel();

            _queue?.Dispose();
            _source?.Dispose();
        }
    }
}
