using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.Extensions.Collections
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Identity function for debug purposes. The collection will be passed to the supplied function
        /// so it could be observed.
        /// </summary>
        /// <typeparam name="T">Collection type.</typeparam>
        /// <param name="e">The collection.</param>
        /// <param name="fn">The function to which the collection will be passed.</param>
        /// <returns>Unchanged collection.</returns>
        public static IEnumerable<T> Identity<T>(this IEnumerable<T> e, Func<IEnumerable<T>, IEnumerable<T>> fn)
        {
            fn(e);
            return e;
        }


        /// <summary>
        /// Wraps this object instance into an IEnumerable&lt;T&gt;
        /// consisting of a single item.
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="item"> The instance that will be wrapped. </param>
        /// <returns> An IEnumerable&lt;T&gt; consisting of a single item. </returns>
        /// <see cref="https://stackoverflow.com/questions/1577822/passing-a-single-item-as-ienumerablet" />
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Go over a collection and perform an action on each item
        /// </summary>
        /// <typeparam name="T"> Type of the object. </typeparam>
        /// <param name="collection"> The Collection. </param>
        /// <param name="action"> The action to perform on each collection item. </param>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) throw new NullReferenceException(nameof(collection));
            foreach (var item in collection)
            {
                action(item);
            }
        }

    }
}
