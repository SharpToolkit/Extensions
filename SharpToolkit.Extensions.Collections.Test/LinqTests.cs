﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace SharpToolkit.Extensions.Collections.Test
{
    [TestClass]
    public class LinqTests
    {
        [TestMethod]
        [DataRow(new[] { false, false, false, false }, -1)]
        [DataRow(new[] { false, false, true, false }, 0)]
        [DataRow(new[] { true, false, false, false }, 0)]
        [DataRow(new[] { true, true, false, false }, 0)]
        [DataRow(new[] { true, true, true, true }, 1)]
        public void Linq_SwitchByTests(bool[] bools, int response)
        {
            Assert.AreEqual(response, bools.SwitchBy(x => x, () => 1, () => 0, () => -1));
        }
    }
}
