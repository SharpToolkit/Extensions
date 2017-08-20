using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpToolkit.Extensions.Diagnostics;
using System.Threading;
using System.Linq;

namespace SharpToolkit.Extensions.Diagnostics.Test
{
    [TestClass]
    public class SynchronizationContractTests
    {
        class TestClass
        {
            public bool value = true;

            public void Method()
            {
                SynchronizationContract.Enter(this, 1);

                // Imitate work
                var r = 0;

                for (var i = 0; i < 10000; i += 1)
                {
                    if (SynchronizationContract.CurrentEntries(this) > 1)
                        throw new AssertFailedException("More than one thread is in the loop");
                    r += i;
                }

                SynchronizationContract.Exit(this);
            }
        }

        [TestMethod]
        public void SynchronizationContract_Correct_Simple()
        {
            var thing = new TestClass();
            thing.Method();
        }

        [TestMethod]
        [ExpectedException(typeof(SynchronizationException))]
        public void SynchronizationContract_Overflow_Simple()
        {
            var thing = new TestClass();
            SynchronizationContract.Enter(thing, 1);

            thing.Method();
        }

        [TestMethod]
        public void SynchronizationContract_Overflow_Multithreaded()
        {
            var overflowed = false;
            var totalEntries = 0;

            var threads = new Thread[2];

            for (var i = 0; i < threads.Length; i += 1)
                threads[i] = new Thread(() =>
                {
                    try
                    {
                        Interlocked.Increment(ref totalEntries);
                        SynchronizationContract.Enter(threads, 1);
                        Thread.Sleep(1000);
                    }
                    catch (SynchronizationException)
                    {
                        overflowed = true;
                    }

                });

            foreach (var t in threads)
                t.Start();

            while (threads.Any(x => x.IsAlive))
            {
                threads.FirstOrDefault(x => x.IsAlive)?.Join();
            }

            Assert.IsTrue(overflowed);
            Assert.AreEqual(totalEntries, 2);
        }

        [TestMethod]
        public void SynchronizationContract_Correct_Multithreaded()
        {
            var overflowed = false;

            var threads = new Thread[2];

            for (var i = 0; i < threads.Length; i += 1)
                threads[i] = new Thread(() =>
                {
                    // Try-catch still needed, as MsTest can't handle
                    // exceptions in threads other than the calling
                    // thread.
                    try
                    {
                        lock (threads)
                        {
                            SynchronizationContract.Enter(threads, 1);
                            Thread.Sleep(1000);
                            SynchronizationContract.Exit(threads);
                        }
                    }
                    catch (SynchronizationException)
                    {
                        overflowed = true;
                    }

                });

            foreach (var t in threads)
                t.Start();

            while (threads.Any(x => x.IsAlive))
            {
                threads.FirstOrDefault(x => x.IsAlive)?.Join();
            }

            Assert.IsFalse(overflowed);
        }

        [TestMethod]
        public void SynchronizationContract_Condition_Correct()
        {
            SynchronizationContract.Enter(this, 1, () => true);
            SynchronizationContract.Exit(this);
        }

        [TestMethod]
        [ExpectedException(typeof(SynchronizationException))]
        public void SynchronizationContract_Condition_IncorrectEntry()
        {
            SynchronizationContract.Enter(this, 1, () => false);
            SynchronizationContract.Exit(this);
        }

        [TestMethod]
        [ExpectedException(typeof(SynchronizationException))]
        public void SynchronizationContract_Condition_IncorrectExit()
        {
            var thing = new TestClass { value = true };

            try
            {
                SynchronizationContract.Enter(this, 1, () => thing.value);
            }
            catch
            {
                Assert.Fail();
            }

            thing.value = false;

            SynchronizationContract.Exit(this);
        }
    }
}
