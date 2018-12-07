﻿using System;

namespace ThreadWorker.Arguments
{
    public class WorkerStatusArgs : WorkerArgs
    {
        public int JobProgress { get; set; }
        public int TotalProgress { get; set; }
        public string Title { get; set; }
        public DateTime DateTime { get; set; }
    }
}