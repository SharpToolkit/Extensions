using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpToolkit.Extensions.Collections
{
    /// <summary>
    /// Comparer that uses passed lambda as the comparison function.
    /// </summary>
    /// <typeparam name="T">The type that will be compared.</typeparam>
    public class LambdaComparer<T> : IComparer<T>, IComparer
    {
        private readonly Func<T, T, int> compareFn;

        public LambdaComparer(Func<T, T, int> compareFn)
        {
            this.compareFn = compareFn;
        }
        
        public int Compare(T x, T y)
        {
            return this.compareFn(x, y);
        }

        public int Compare(object x, object y)
        {
            if (x is T == false)
                throw new ArgumentException($"{nameof(x)} is not an instance of {typeof(T).FullName}");

            if (y is T == false)
                throw new ArgumentException($"{nameof(y)} is not an instance of {typeof(T).FullName}");

            return this.Compare((T)x, (T)y);
        }
    }
}
