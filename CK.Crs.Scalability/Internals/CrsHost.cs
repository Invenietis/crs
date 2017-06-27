using CK.Core;
using System;
using System.Collections.Generic;
using System.Threading;
using CK.Crs.Runtime;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace CK.Crs.Scalability.Internals
{
    class CrsHost : ICrsHost
    {
        private IActivityMonitor _monitor;
        private IServiceProvider _services;
        private IList<ICommandListener> _listeners;
        private IList<ICommandResponseDispatcher> _dispatchers;
        readonly ICrsHandler _handler;
        public CrsHost(IActivityMonitor monitor, ICrsHandler handler, IServiceProvider services, IList<ICommandListener> listeners, IList<ICommandResponseDispatcher> dispatchers)
        {
            _monitor = monitor;
            _handler = handler;
            _services = services;
            _listeners = listeners;
            _dispatchers = dispatchers;
        }

        public void WaitForMessages(CancellationToken token)
        {
            ICommandWorkerQueue workerQueue =  new Internals.InProcessWorkerQueue(_handler, new Internals.DefaultCommandRequestFactory() );

            foreach (var listener in _listeners)
            {
                _monitor.Trace().Send("Calling Receive for listener {0} ", listener.GetType().Name);
                try
                {
                    Task.Run(() => listener.Receive(workerQueue, token));
                }
                catch (Exception ex)
                {
                    _monitor.Error().Send(ex);
                }
            }

            workerQueue.JobCompleted += OnJobCompleted;

            try
            {
                workerQueue.Start();

                _monitor.Trace().Send("Waiting for messages...");
                token.WaitHandle.WaitOne();

                _monitor.Trace().Send("Crs Host shutdowning...");
            }
            finally
            {
                workerQueue.JobCompleted -= OnJobCompleted;
                workerQueue.Dispose();
            }
        }


        async void OnJobCompleted(object sender, JobCompletedEventArgs args)
        {
            foreach (var dispatcher in _dispatchers)
            {
                await dispatcher
                    .DispatchAsync(args.Job.Monitor, args.Job.MetaData.CallbackId, args.ResponseBuilder)
                    .ConfigureAwait(false);
            }
        }
    }


}
