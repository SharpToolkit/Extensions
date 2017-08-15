using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.Extensions
{
    /// <summary>
    /// Marks class as being able to safely copy itself.
    /// </summary>
    /// <typeparam name="T">The type of the class.</typeparam>
    public interface ICopyable<T>
    {
        /// <summary>
        /// Safely copies the object.
        /// </summary>
        /// <returns></returns>
        T Copy();
    }
}
