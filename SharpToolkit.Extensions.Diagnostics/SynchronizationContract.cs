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

        private static void enterUnsafeUnchecked(object obj)
        {
            if (false == entries.ContainsKey(obj))
                entries.Add(obj, 0);

            entries[obj] = entries[obj] + 1;
        }

        private static void enterUnsafe(object obj, int limit)
        {
            enterUnsafeUnchecked(obj);

            if (entries[obj] > limit)
            {
                throw new SynchronizationException($"More than {limit} threads have entered the {obj.ToString()} code block.");
            }
        }

        private static void enterUnsafe(object obj, Func<bool> predicate)
        {
            if (false == predicate())
                throw new SynchronizationException("Conditional entry failed.");

            conditions.Add(obj, predicate);
        }

        /// <summary>
        /// Registers an entry to a code block with limited participants.
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

        /// <summary>
        /// Registers an entry to a code block with a condition.
        /// </summary>
        /// <param name="obj">The object that is used to identify the block.</param>
        /// <param name="predicate">The condition to code block entry.</param>
        /// <exception cref="SynchronizationException">
        /// Thrown when the condition is not met.
        /// </exception>
        [Conditional("DEBUG")]
        public static void Enter(object obj, Func<bool> predicate)
        {
            lock (syncRoot)
            {
                enterUnsafeUnchecked(obj);
                enterUnsafe(obj, predicate);
            }
        }

        /// <summary>
        /// Registers an entry to a code block with limited participants and a condition.
        /// </summary>
        /// <param name="obj">The object that is used to identify the block.</param>
        /// <param name="limit">Maximum namber of threads allowed inside the block.</param>
        /// <param name="predicate">The condition to code block entry.</param>
        /// <exception cref="SynchronizationException">
        /// Thrown when the number of threads inside the code block is higher than the limit
        /// or when the condition is not met.
        /// </exception>
        [Conditional("DEBUG")]
        public static void Enter(object obj, int limit, Func<bool> predicate)
        {
            lock (syncRoot)
            {
                enterUnsafe(obj, limit);
                enterUnsafe(obj, predicate);
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
        /// <exception cref="SynchronizationException">
        /// When number of exiting threads exceeds number of entering threads
        /// or when the condition is not met, when available.
        /// </exception>
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
