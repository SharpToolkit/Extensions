using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpToolkit.Extensions.Collections;

namespace SharpToolkit.Extensions.Collections.Test
{
    [TestClass]
    public class LambdaEqualityComparerTests
    {
        class CompareTarget
        {
            public int Num;
            public string Str;

            public CompareTarget(int num, string str)
            {
                this.Num = num;
                this.Str = str;
            }
        }

        [TestMethod]
        public void CompareEquals()
        {
            var x = new CompareTarget(1, "str");
            var y = new CompareTarget(1, "str");

            var comparer = new LambdaEqualityComparer<CompareTarget>(
                (_x, _y) => _x.Num == _y.Num && _x.Str == _y.Str);

            Assert.IsTrue(comparer.Equals(x, y));
            Assert.IsTrue(comparer.Equals((object)x, (object)y));
        }

        [TestMethod]
        public void CompareNotEquals()
        {
            var x = new CompareTarget(1, "str");
            var y = new CompareTarget(2, "otherStr");

            var comparer = new LambdaEqualityComparer<CompareTarget>(
                (_x, _y) => _x.Num == _y.Num && _x.Str == _y.Str);

            Assert.IsFalse(comparer.Equals(x, y));
            Assert.IsFalse(comparer.Equals((object)x, (object)y));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CompareWrongTypes()
        {
            var x = new CompareTarget(1, "str");
            var y = new object();

            var comparer = new LambdaEqualityComparer<CompareTarget>(
                (_x, _y) => _x.Num == _y.Num && _x.Str == _y.Str);

            comparer.Equals((object)x, y);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void CheckHashCode()
        {
            var x = new CompareTarget(1, "str");
            var y = new object();

            var comparer = new LambdaEqualityComparer<CompareTarget>(
                (_x, _y) => _x.Num == _y.Num && _x.Str == _y.Str);

            Assert.AreEqual(comparer.GetHashCode((object)x), x.GetHashCode());

            comparer.GetHashCode(y);
        }
    }
}
