using System;

namespace ThreadWorker.Code
{
    /// <summary>
    /// Shell to transfer the title and task.
    /// </summary>
    public sealed class WorkContainer
    {
        /// <summary>
        /// The title of the task.
        /// </summary>
        public string Title { get; private set; }
        /// <summary>
        /// Delegate with the body of the task.
        /// </summary>
        public Action<WorkManager, TokenManager> Action { get; set; }

        private WorkContainer() { }
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="title">The title of the task.</param>
        /// <param name="action">Delegate with the body of the task.</param>
        public WorkContainer(string title, Action<WorkManager, TokenManager> action)
        {
            Title = title;
            Action = action;
        }
    }
}
