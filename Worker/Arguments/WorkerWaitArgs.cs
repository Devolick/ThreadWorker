using System;
using ThreadWorker.Code;

namespace ThreadWorker.Arguments
{
    /// <summary>
    /// Arg
    /// </summary>
    public class WorkerWaitArgs : WorkerStatusArgs
    {
        /// <summary>
        /// True if tasks had stoped.
        /// </summary>
        public bool Pause { get; set; }
    }
}
