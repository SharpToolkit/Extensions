using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Executes one of three passed functions depending on the
        /// relative amount of items that satisfy the predicate.
        /// </summary>
        /// <typeparam name="T">Type of the enumerable.</typeparam>
        /// <typeparam name="U">Return type.</typeparam>
        /// <param name="e">The enumerable.</param>
        /// <param name="predicate">The predicate which switches the function.</param>
        /// <param name="allFn">Function to execute when all items satisfy the predicate.</param>
        /// <param name="someFn">Function to execute when some of the items satisfy the predicate.</param>
        /// <param name="noneFn">Function to execute when none of the items satisfy the predicate.</param>
        /// <returns>The value that was returned from one of the functions.</returns>
        public static U SwitchByRelative<T, U>(
            this IEnumerable<T> e,
            Func<T, bool> predicate,
            Func<U> allFn,
            Func<U> someFn,
            Func<U> noneFn)
        {
            return SwitchByRelative(
                e,
                predicate,
                () => allFn(),
                _ => someFn(),
                _ => noneFn()
                );
        }

        /// <summary>
        /// Executes one of three passed functions depending on the
        /// relative amount of items that satisfy the predicate.
        /// Items that did not satisfy the condition will be passed
        /// into the switch function.
        /// </summary>
        /// <typeparam name="T">Type of the enumerable.</typeparam>
        /// <typeparam name="U">Return type.</typeparam>
        /// <param name="e">The enumerable.</param>
        /// <param name="predicate">The predicate which switches the function.</param>
        /// <param name="allFn">Function to execute when all items satisfy the predicate.</param>
        /// <param name="someFn">Function to execute when some of the items satisfy the predicate.</param>
        /// <param name="noneFn">Function to execute when none of the items satisfy the predicate.</param>
        /// <returns>The value that was returned from one of the functions.</returns>
        public static U SwitchByRelative<T, U>(
            this IEnumerable<T> e,
            Func<T, bool> predicate,
            Func<U> allFn,
            Func<IEnumerable<T>, U> someFn,
            Func<IEnumerable<T>, U> noneFn)
        {
            int trues = 0;
            int count = 0;

            LinkedList<T> untrues = new LinkedList<T>();

            foreach (var item in e)
            {
                count += 1;
                if (predicate(item))
                    trues += 1;
                else
                    untrues.AddLast(item);
            }

            // TODO: Throw excetion on zero items in collection. Undefined behaviour.

            if (trues == count)
                return allFn();

            if (trues == 0)
                return noneFn(untrues);

            return someFn(untrues);
        }

        /// <summary>
        /// Returns true if the collection contains a single item only.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="e">The enumerable.</param>
        /// <returns>True if the collection contains a songle ite,</returns>
        public static bool OnlyOne<T>(this IEnumerable<T> e)
        {
            int i = 0;
            foreach (var item in e)
            {
                i += 1;
                if (i > 1)
                    break;
            }

            return 1 == i;
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
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if (collection == null) throw new NullReferenceException(nameof(collection));
            foreach (var item in collection)
            {
                action(item);

                yield return item;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> collection)
        {
            return collection.Aggregate(new LinkedList<T>(), (aggr, cur) => { aggr.AddLast(cur); return aggr; });
        }

    }
}
