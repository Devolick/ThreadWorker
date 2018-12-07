using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadWorker.Code
{
    public sealed class WorkContainer
    {
        public string Title { get; private set; }
        public Action<WorkManager, Token> Action { get; set; }

        private WorkContainer() { }
        public WorkContainer(string title, Action<WorkManager, Token> action)
        {
            Title = title;
            Action = action;
        }
    }
}
