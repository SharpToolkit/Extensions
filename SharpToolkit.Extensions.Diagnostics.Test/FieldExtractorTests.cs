﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SharpToolkit.Extensions.Diagnostics.Test
{
    [TestClass]
    public class FieldExtractorTests
    {
        [TestMethod]
        public void FieldExtractor_Extract()
        {
            var target1 = new TestTargetA();
            var target2 = new TestTargetA();


            var extracted1 = new FieldExtractor(typeof(TestTargetA)).Extract(target1);
            var extracted2 = new FieldExtractor(typeof(TestTargetA)).Extract(target2);

            foreach (var x in extracted1)
                foreach (var y in extracted2)
                    if (object.ReferenceEquals(x, y))
                        Debugger.Break();

            Assert.IsFalse(extracted1.Any(x => extracted2.Any(y => object.ReferenceEquals(x, y))));
        }
    }
}
