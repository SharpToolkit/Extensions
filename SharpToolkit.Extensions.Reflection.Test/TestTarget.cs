using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.Extensions.Reflection.Test
{
    class TestTarget
    {
        public string Str1 { get; set; }
        public string Str2 { get; set; }

        public TestTarget(string str1)
        {
            this.Str1 = str1;
        }

        public TestTarget(string str1, string str2)
        {
            this.Str1 = str1;
            this.Str2 = str2;
        }
    }
}
