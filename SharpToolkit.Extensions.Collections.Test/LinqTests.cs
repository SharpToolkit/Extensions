using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpToolkit.Extensions.Collections.Test
{
    [TestClass]
    public class LinqTests
    {
        [TestMethod]
        [DataRow(new bool[] {  }, 1)]
        [DataRow(new[] { false, false, false, false }, -1)]
        [DataRow(new[] { false, false, true, false }, 0)]
        [DataRow(new[] { true, false, false, false }, 0)]
        [DataRow(new[] { true, true, false, false }, 0)]
        [DataRow(new[] { true, true, true, true }, 1)]
        public void Linq_SwitchByTests(bool[] bools, int response)
        {
            Assert.AreEqual(response, bools.SwitchByRelative(x => x, () => 1, () => 0, () => -1));
        }

        [TestMethod]
        [DataRow(new[] { false, false, false, false }, 4)]
        [DataRow(new[] { false, false, true, false }, 3)]
        [DataRow(new[] { true, false, false, false }, 3)]
        [DataRow(new[] { true, true, false, false }, 2)]
        [DataRow(new[] { true, true, true, true }, 0)]
        public void Linq_SwitchBy_WithEnumerablesTests(bool[] bools, int response)
        {
            var items = 
                bools.SwitchByRelative(
                    x => x,
                    () => Enumerable.Empty<bool>(),
                    x => x,
                    x => x);

            Assert.AreEqual(response, items.Count());
        }

        [TestMethod]
        [DataRow(new int[] {  }, false)]
        [DataRow(new[] { 0 }, true)]
        [DataRow(new[] { 0, 1 }, false)]
        [DataRow(new[] { 0, 1, 2 }, false)]
        public void Linq_OnlyOne(int [] ints, bool response)
        {
            Assert.AreEqual(response, ints.OnlyOne());
        }

        private class ForEachTestClass
        {
            public int Number { get; set; }
        }

        [TestMethod]
        public void Linq_ForEach()
        {
            var list = new[]
            {
                new ForEachTestClass { Number = 1 },
                new ForEachTestClass { Number = 2 },
                new ForEachTestClass { Number = 3 }
            };

            list.ForEach(x => x.Number++);

            Assert.AreEqual(2, list[0].Number);
            Assert.AreEqual(3, list[1].Number);
            Assert.AreEqual(4, list[2].Number);
        }
    }
}
