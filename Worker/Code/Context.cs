using System;
using System.Collections.Generic;
using System.Linq;

namespace ThreadWorker.Code
{
    public class Context
    {
        private LinkedList<KeyValuePair<string, dynamic>> values;

        public Context()
        {
            values = new LinkedList<KeyValuePair<string, dynamic>>();
        }

        /// <summary>
        /// Check page value exist.
        /// </summary>
        /// <param name="pageNameOf">Use nameof(pageType) to find page data.</param>
        /// <returns>Returns true if page exist.</returns>
        public bool HasValue(string pageNameOf)
        {
            return values.Any(a => a.Key == pageNameOf);
        }

        /// <summary>
        /// Add page transfer data.
        /// </summary>
        /// <param name="pageNameOf">Use nameof(pageType) to find page data.</param>
        /// <param name="value">Transfer object data for current page.</param>
        public void AppendValue(string pageNameOf, dynamic value)
        {
            if (values.Any(a => a.Key == pageNameOf))
                throw new DuplicateWaitObjectException("This page is already in the list.");

            values.AddLast(new KeyValuePair<string, dynamic>(pageNameOf, value));
        }

        public void RemoveValue(string pageNameOf)
        {
            if (values.Any(a => a.Key == pageNameOf))
            {
                KeyValuePair<string, dynamic> value =
                    values.First(f => f.Key == pageNameOf);
                values.Remove(value);
            }
        }

        /// <summary>
        /// Get page transfer data.
        /// </summary>
        /// <typeparam name="T">Explicit current data to T type.</typeparam>
        /// <param name="pageNameOf">Use nameof(pageType) to find page data.</param>
        /// <returns>Returns page transfer data between pages.</returns>
        public T GetValue<T>(string pageNameOf)
            where T : class
        {
            if (values.Any(a => a.Key == pageNameOf))
                return (T)values.First(f => f.Key == pageNameOf).Value;

            return null;
        }
    }
}
