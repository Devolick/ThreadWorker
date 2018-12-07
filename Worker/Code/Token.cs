using System;
using System.Reflection;

namespace ThreadWorker.Code
{
    public sealed class Token
    {
        public int JobIndex { get; set; }
        public int ProgressStep { get; set; }
        public int ProgressIndex { get; set; }

        public Token() { }
    }
}
