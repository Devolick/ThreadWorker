using System;
using System.Reflection;

namespace ThreadWorker.Code
{
    /// <summary>
    /// Class for transfer between tasks. Used as a notebook in case of an error.
    /// </summary>
    public sealed class Token
    {
        /// <summary>
        /// Indicates the index of the current task.
        /// </summary>
        public int TaskIndex { get; set; }
        /// <summary>
        /// Flexible class for data entry.
        /// </summary>
        public Context Context { get; set; }

        /// <summary>
        /// Ctor
        /// </summary>
        public Token() { }
    }
}
