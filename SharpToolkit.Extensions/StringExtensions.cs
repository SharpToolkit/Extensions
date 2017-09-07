using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.Extensions
{
    public static class StringExtensions
    {
        public static string TrimTo(this string str, int length)
        {
            length = length > str.Length ? str.Length : length;

            return str.Substring(0, length);
        }
    }
}
