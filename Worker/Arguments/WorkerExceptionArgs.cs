using System;

namespace ThreadWorker.Arguments
{
    /// <summary>
    /// Arg
    /// </summary>
    public class WorkerExceptionArgs : WorkerArgs
    {
        /// <summary>
        /// Represent exception for worker thread.
        /// </summary>
        public Exception Exception { get; set; }
    }
}
