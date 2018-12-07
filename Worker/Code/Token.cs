using System;
using System.Reflection;

namespace ThreadWorker.Code
{
    public sealed class Token
    {
        public int TaskIndex { get; set; }
        public Context Context { get; set; }

        public Token() { }
    }
}
