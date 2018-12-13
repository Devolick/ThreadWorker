using System;
using ThreadWorker.Code;

namespace ThreadWorker.Arguments
{
    /// <summary>
    /// Arg
    /// </summary>
    public class WorkerArgs : EventArgs
    {
        /// <summary>
        /// Transfer data between tasks.
        /// </summary>
        public Token Token { get; set; }
    }
}
