using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpToolkit.Extensions.Diagnostics.Test
{
    class TestTargetA
    {
        private Guid id;

        private int privateInt;
        private string privateString;
        private TestTargetB privateTargetB;

        public int PublicInt;
        public string PublicString;
        public TestTargetB PublicTargetB;

        public int PublicIntProp => this.privateInt;
        public string PublicStringProp => this.privateString;
        public TestTargetB PublicTargetBProp => this.privateTargetB;

        public int PublicIntAutoProp { get; set; }
        public string PublicStringAutoProp { get; set; }
        public TestTargetB publicTargetBAutoProp { get; set; }

        public TestTargetB[] PublicTestTargetBArray { get; set; }

        private DictionaryTestTarget dictionaryTestTarget;

        //public IEnumerable<int> IntEnumerable { get; }
        //public IEnumerable<String> StringEnumerable { get; }
        //public IEnumerable<TestTargetB> TargetBEnumerable { get; }

        public TestTargetA()
        {
            id = Guid.NewGuid();

            privateString = "qwerty";
            privateTargetB = new TestTargetB();

            PublicString = "qwerty";
            PublicTargetB = new TestTargetB();

            PublicStringAutoProp = "qwerty";
            publicTargetBAutoProp = new TestTargetB();

            PublicTestTargetBArray = new[] { new TestTargetB(), new TestTargetB() };

            dictionaryTestTarget = new DictionaryTestTarget();
        }

        public override string ToString()
        {
            return $"TestTargetA, {id}";
        }
    }

    class TestTargetB
    {
        private Guid id;

        public string String { get; set; }
        public object Object { get; set; }

        public TestTargetB()
        {
            id = Guid.NewGuid();
            this.Object = new object();
        }

        public override string ToString()
        {
            return $"TestTargetB, {id}";
        }
    }

    class DictionaryTestTarget
    {
        private Guid id;
        private IDictionary<Guid, TestTargetB> dictionary;

        public DictionaryTestTarget()
        {
            id = Guid.NewGuid();

            var commonTarget = new TestTargetB();

            dictionary = Enumerable
                .Range(0, 10)
                .ToDictionary(x => Guid.NewGuid(), x => new TestTargetB())
                .Union(
                    Enumerable
                    .Range(0, 10)
                    .ToDictionary(x => Guid.NewGuid(), x => commonTarget)
                )
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }

    class RecursiveTargetOuter
    {
        public RecursiveTargetInner Inner { get; set; }
    }

    class RecursiveTargetInner
    {
        public RecursiveTargetOuter Outer { get; set; }
    }
}
