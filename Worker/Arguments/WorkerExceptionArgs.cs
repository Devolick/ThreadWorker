using System;

namespace ThreadWorker.Arguments
{
    public class WorkerExceptionArgs : WorkerArgs
    {
        public Exception Exception { get; set; }
    }
}
