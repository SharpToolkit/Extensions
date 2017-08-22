using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.Extensions.Collections
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Identity function for debug purposes. The collection will be passed to the supplied action
        /// so it could be observed.
        /// </summary>
        /// <typeparam name="T">Collection type.</typeparam>
        /// <param name="e">The collection.</param>
        /// <param name="action">Action to which the collection will be passed.</param>
        /// <returns>Unchanged collection.</returns>
        public static IEnumerable<T> Identity<T>(this IEnumerable<T> e, Action<IEnumerable<T>> action)
        {
            action(e);
            return e;
        }
    }
}
