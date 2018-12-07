using System;

namespace ThreadWorker.Code
{
    public sealed class WorkContainer
    {
        public string Title { get; private set; }
        public Action<WorkManager, TokenManager> Action { get; set; }

        private WorkContainer() { }
        public WorkContainer(string title, Action<WorkManager, TokenManager> action)
        {
            Title = title;
            Action = action;
        }
    }
}
