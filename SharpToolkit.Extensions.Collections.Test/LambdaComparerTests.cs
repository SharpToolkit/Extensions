using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SharpToolkit.Extensions.Collections.Test
{
    [TestClass]
    public class LambdaComparerTests
    {
        class CompareTarget
        {
            public int Num;

            public CompareTarget(int num)
            {
                this.Num = num;
            }
        }

        [TestMethod]
        public void Sort()
        {
            var comparer = new LambdaComparer<CompareTarget>((x, y) =>
            {
                if (x.Num < y.Num)
                    return -1;
                if (x.Num > y.Num)
                    return 1;

                return 0;
            });

            var list = new List<CompareTarget>()
            {
                new CompareTarget(3),
                new CompareTarget(6),
                new CompareTarget(0),
                new CompareTarget(2),
                new CompareTarget(1),
                new CompareTarget(4),
                new CompareTarget(5)
            };

            list.Sort(comparer);

            for (int i = 0; i < list.Count; i += 1)
                Assert.AreEqual(i, list[i].Num);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WrongType()
        {
            var comparer = new LambdaComparer<CompareTarget>((x, y) =>
            {
                if (x.Num < y.Num)
                    return -1;
                if (x.Num > y.Num)
                    return 1;

                return 0;
            });

            var _x = new CompareTarget(1);
            var _y = new object();

            comparer.Compare(_x, _y);
        }
    }

    
}
