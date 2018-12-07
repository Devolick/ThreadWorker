using System;
using ThreadWorker.Code;

namespace ThreadWorker.Arguments
{
    public class WorkerArgs : EventArgs
    {
        public Token Token { get; set; }
    }
}
