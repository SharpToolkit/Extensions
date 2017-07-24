using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpToolkit.Extensions;

namespace SharpToolkit.Extensions.Test
{
    [TestClass]
    public class Exceptions
    {
        [TestMethod]
        public void UnwrapException()
        {
            var ex = 
                new AggregateException(
                    "Outer aggregate.",
                    new AggregateException(
                        "Inner aggregate.",
                        new DivideByZeroException("Outer zero."),
                        new InvalidOperationException("Invalid operation.", new DivideByZeroException("Inner zero."))
                    )
                );

            var exs = ex.UnwrapException();


            Assert.IsTrue(exs[0].Exception.Message.StartsWith("Outer aggregate."));
            Assert.AreEqual(exs[0].Level, 0);
            Assert.IsInstanceOfType(exs[0].Exception, typeof(AggregateException));

            Assert.AreEqual(exs[1].Exception.Message, "Outer zero.");
            Assert.AreEqual(exs[1].Level, 1);
            Assert.IsInstanceOfType(exs[1].Exception, typeof(DivideByZeroException));

            Assert.AreEqual(exs[2].Exception.Message, "Invalid operation.");
            Assert.AreEqual(exs[2].Level, 1);
            Assert.IsInstanceOfType(exs[2].Exception, typeof(InvalidOperationException));

            Assert.AreEqual(exs[3].Exception.Message, "Inner zero.");
            Assert.AreEqual(exs[3].Level, 2);
            Assert.IsInstanceOfType(exs[3].Exception, typeof(DivideByZeroException));
        }
    }
}
