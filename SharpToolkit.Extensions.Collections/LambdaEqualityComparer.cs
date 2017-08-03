using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.Extensions.Collections
{
    /// <summary>
    /// An equality comparer that uses passed lambda as the comparison function.
    /// </summary>
    /// <typeparam name="T">The type that will be compared.</typeparam>
    public class LambdaEqualityComparer<T> : IEqualityComparer<T>, IEqualityComparer
    {
        private readonly Func<T, T, bool> compareFn;

        public LambdaEqualityComparer(Func<T, T, bool> compareFn)
        {
            this.compareFn = compareFn;
        }

        public new bool Equals(object x, object y)
        {
            if (x is T == false)
                throw new ArgumentException($"{nameof(x)} is not an instance of {typeof(T).FullName}");

            if (y is T == false)
                throw new ArgumentException($"{nameof(y)} is not an instance of {typeof(T).FullName}");

            return this.Equals((T)x, (T)y);
        }

        public bool Equals(T x, T y)
        {
            if (x == null ||
                y == null)
                return false;

            if (object.ReferenceEquals(x, y))
                return true;

            return this.compareFn(x, y);
        }

        public int GetHashCode(object obj)
        {
            if (obj is T == false)
                throw new ArgumentException($"{nameof(obj)} is not an instance of {typeof(T).FullName}");

            return this.GetHashCode((T)obj);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
