using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpToolkit.Extensions.Diagnostics
{
    /// <summary>
    /// Contains methods for tracing access to blocks of code.
    /// </summary>
    public class SynchronizationContract
    {
        private static object syncRoot;

        private static Dictionary<object, int> entries;
        private static Dictionary<object, Func<bool>> conditions;

        static SynchronizationContract()
        {
            syncRoot = new object();
            Reset();
        }

        private static void enterUnsafe(object obj, int limit)
        {
            if (false == entries.ContainsKey(obj))
                entries.Add(obj, 0);

            entries[obj] = entries[obj] + 1;

            if (entries[obj] > limit)
            {
                throw new SynchronizationException($"More than {limit} threads have entered the {obj.ToString()} code block.");
            }
        }

        /// <summary>
        /// Registers an entry to a code block.
        /// </summary>
        /// <param name="obj">The object that is used to identify the block.</param>
        /// <param name="limit">Maximum namber of threads allowed inside the block.</param>
        /// <exception cref="SynchronizationException">
        /// Thrown when the number of threads inside the code block is higher than the limit.
        /// </exception>
        [Conditional("DEBUG")]
        public static void Enter(object obj, int limit)
        {
            lock (syncRoot)
            {
                enterUnsafe(obj, limit);
            }
        }

        [Conditional("DEBUG")]
        public static void Enter(object obj, int limit, Func<bool> predicate)
        {
            lock (syncRoot)
            {
                if (false == predicate())
                    throw new SynchronizationException("Conditional entry failed.");

                enterUnsafe(obj, limit);

                conditions.Add(obj, predicate);
            }
        }

        /// <summary>
        /// Method used for debugging purposes.
        /// </summary>
        /// <param name="obj">The object that is used to identify the block.</param>
        /// <returns>Number of threads that are currently inside the block.</returns>
        internal static int CurrentEntries(object obj)
        {
            var r = 0;

            lock (syncRoot)
            {
                r = entries[obj];
            }

            return r;
        }

        /// <summary>
        /// Registers an exit from a code block.
        /// </summary>
        /// <param name="obj">The object that is used to identify the block.</param>
        [Conditional("DEBUG")]
        public static void Exit(object obj)
        {
            lock (syncRoot)
            {
                entries[obj] = entries[obj] - 1;

                if (entries[obj] < 0)
                    throw new SynchronizationException($"Code block {obj.ToString()} has exited more times than entered.");

                if (conditions.ContainsKey(obj))
                {
                    if (false == conditions[obj]())
                        throw new SynchronizationException("Conditional exit failed");

                    conditions.Remove(obj);
                }
            }
        }

        /// <summary>
        /// Thread unsafe. Resets the counters.
        /// </summary>
        public static void Reset()
        {
            entries = new Dictionary<object, int>();
            conditions = new Dictionary<object, Func<bool>>();
        }
    }
}
