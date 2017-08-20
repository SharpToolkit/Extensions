using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.Extensions.Diagnostics
{
    public class SynchronizationException : Exception
    {
        public SynchronizationException(string msg) : base(msg) { }
    }
}
