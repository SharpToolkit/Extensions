using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.Extensions
{
    public static class ExceptionExtensions
    {
        public class ExceptionStackItem
        {
            public Exception Exception { get; }
            public int Level { get; }

            public ExceptionStackItem(Exception exception, int level)
            {
                this.Exception = exception;
                this.Level     = level;
            }
        }

        public class ExceptionStack : List<ExceptionStackItem> { }

        /// <summary>
        /// Returns an easy to log flattened exception stack.
        /// </summary>
        /// <remarks>
        /// The returned list is a indented list of exceptions. Each exception
        /// comes with an indentation level. All child exceptions are one
        /// level deeper.
        /// </remarks>
        /// <param name="e">The exception in question.</param>
        /// <returns>Flattened exception stack.</returns>
        public static ExceptionStack UnwrapException(this Exception e)
        {
            var stack = new ExceptionStack();

            unwrapExceptionImpl(stack, e, 0);

            return stack;
        }

        private static void unwrapExceptionImpl(ExceptionStack stack, Exception exception, int level)
        {
            if (exception == null)
                return;

            if (exception is AggregateException aggr)
            {
                stack.Add(new ExceptionStackItem(exception, level));

                foreach (var ex in aggr.Flatten().InnerExceptions)
                    unwrapExceptionImpl(stack, ex, level + 1);
            }

            else
            {
                stack.Add(new ExceptionStackItem(exception, level));
                unwrapExceptionImpl(stack, exception.InnerException, level + 1);
            }
        }
    }
}
