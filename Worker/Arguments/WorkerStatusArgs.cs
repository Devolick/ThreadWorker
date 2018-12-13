using System;

namespace ThreadWorker.Arguments
{
    /// <summary>
    /// Arg
    /// </summary>
    public class WorkerStatusArgs : WorkerArgs
    {
        /// <summary>
        /// Current task progress.
        /// </summary>
        public int TaskProgress { get; set; }
        /// <summary>
        /// Total progress for all tasks.
        /// </summary>
        public int TotalProgress { get; set; }
        /// <summary>
        /// Title from container.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// When task had start.
        /// </summary>
        public DateTime Started { get; set; }
    }
}
