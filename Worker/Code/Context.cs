using System;
using System.Collections.Generic;
using System.Linq;

namespace ThreadWorker.Code
{
    /// <summary>
    /// Class content entries.
    /// </summary>
    public class Context
    {
        private LinkedList<KeyValuePair<string, object>> values;

        /// <summary>
        /// Ctor
        /// </summary>
        public Context()
        {
            values = new LinkedList<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// Find out if there is an entry with the specified name. Use the nameof(workName) operator.
        /// </summary>
        public bool HasValue(string workName)
        {
            return values.Any(a => a.Key == workName);
        }

        /// <summary>
        /// Adds task to the list of values. Use the nameof(workName) operator.
        /// </summary>
        public void AppendValue(string workName, object value)
        {
            if (values.Any(a => a.Key == workName))
                throw new DuplicateWaitObjectException("This page is already in the list.");

            values.AddLast(new KeyValuePair<string, object>(workName, value));
        }

        /// <summary>
        /// Removes a task from the list of values. Use the nameof(workName) operator.
        /// </summary>
        public void RemoveValue(string workName)
        {
            if (values.Any(a => a.Key == workName))
            {
                KeyValuePair<string, object> value =
                    values.First(f => f.Key == workName);
                values.Remove(value);
            }
        }

        /// <summary>
        /// Gets a task from a list of values. Use the nameof(workName) operator.
        /// </summary>
        public T GetValue<T>(string workName)
            where T : class
        {
            if (values.Any(a => a.Key == workName))
                return (T)values.First(f => f.Key == workName).Value;

            return null;
        }
    }
}
