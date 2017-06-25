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

        event EventHandler<JobCompletedEventArgs> JobCompleted;
    }
}
