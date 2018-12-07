using System;
using System.Collections.Generic;
using System.Text;

namespace ThreadWorker.Arguments
{
    public class WorkerExceptionArgs : WorkerArgs
    {
        public Exception Exception { get; set; }
    }
}
