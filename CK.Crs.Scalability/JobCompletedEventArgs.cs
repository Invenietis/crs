using System;
using CK.Crs.Runtime;

namespace CK.Crs.Scalability
{
    public class JobCompletedEventArgs : EventArgs
    {
        public readonly CommandJob Job;
        public readonly CommandResponseBuilder ResponseBuilder;

        public JobCompletedEventArgs(CommandJob job, CommandResponseBuilder responseBuilder )
        {
            this.Job = job;
            this.ResponseBuilder = responseBuilder;
        }
    }
}
