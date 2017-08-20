using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpToolkit.Extensions.Testing
{
    /// <summary>
    /// Contains methods for tracing access to blocks of code.
    /// </summary>
    public class SynchronizationContract
    {
        private static Dictionary<object, int> entries;

        static SynchronizationContract()
        {
            Reset();
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
            lock (entries)
            {
                if (false == entries.ContainsKey(obj))
                    entries.Add(obj, 0);

                entries[obj] = entries[obj] + 1;

                if (entries[obj] > limit)
                {
                    throw new SynchronizationException($"More than {limit} threads have entered the {obj.ToString()} code block.");
                }
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

            lock (entries)
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
            lock (entries)
            {
                entries[obj] = entries[obj] - 1;

                if (entries[obj] < 0)
                    throw new SynchronizationException($"Code block {obj.ToString()} has exited more times than entered.");
            }
        }

        /// <summary>
        /// Thread unsafe. Resets the counters.
        /// </summary>
        public static void Reset()
        {
            entries = new Dictionary<object, int>();
        }
    }
}
