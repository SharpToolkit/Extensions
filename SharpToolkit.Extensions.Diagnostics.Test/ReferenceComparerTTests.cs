using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpToolkit.Extensions.Diagnostics.Test
{
    [TestClass]
    public class ReferenceComparerTTests
    {
        [TestMethod]
        public void ReferenceComparerTTests_Members()
        {
            var target = new TestTargetA();

            var comparer = ReferenceComparer.GetComparer(typeof(TestTargetA));

            Assert.AreEqual(12, comparer.Members.Count());
        }

        [TestMethod]
        public void ReferenceComparerTTests_SameObject()
        {
            var target = new TestTargetA();

            var comparer = ReferenceComparer.GetComparer(typeof(TestTargetA));

            var result = comparer.Compare(target, target);

            Assert.IsTrue(result.Any(x => x.result));
        }

        [TestMethod]
        public void ReferenceComparerTTests_DifferentObjects()
        {
            var target1 = new TestTargetA();
            var target2 = new TestTargetA();

            var comparer = ReferenceComparer.GetComparer(typeof(TestTargetA));

            var result = comparer.Compare(target1, target2);

            Assert.IsTrue(result.All(x => x.result == false));
        }

        [TestMethod]
        public void ReferenceComparerTTests_DifferentObjects_SameInnerObject()
        {
            var target1 = new TestTargetA();
            var target2 = new TestTargetA();

            target2.PublicTargetB.Object = target1.PublicTargetB.Object;
            target1.publicTargetBAutoProp = null;
            target2.publicTargetBAutoProp = null;

            var comparer = ReferenceComparer.GetComparer(typeof(TestTargetA));

            var result = comparer.Compare(target1, target2);

            Assert.IsTrue(result.Count(x => x.result == true) == 1);
        }
    }
}
