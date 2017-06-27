using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CK.Crs.Scalability
{
    public interface ICommandWorkerQueue : IDisposable
    {
        /// <summary>
        /// Adds the given command 
        /// </summary>
        /// <param name="command"></param>
        void Add(CommandJob command);

        /// <summary>
        /// Starts the queue
        /// </summary>
        void Start();

        /// <summary>
        /// Should be raised when a job is completed
        /// </summary>
        event EventHandler<JobCompletedEventArgs> JobCompleted;
    }
}
