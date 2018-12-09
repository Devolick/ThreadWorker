using System;
using ThreadWorker.Code;

namespace ThreadWorker.Arguments
{
    public class WorkerWaitArgs : WorkerStatusArgs
    {
        public bool Pause { get; set; }
    }
}
