using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpToolkit.Extensions.Diagnostics.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharpToolkit.Extensions.Diagnostics.Test.Testing
{
    [TestClass]
    public class MultithreadedTestRigTests
    {
        class TestContext
        {
            public bool CancellationRequested { get; set; }

            public TestContext()
            {
                this.CancellationRequested = false;
            }
        }


        [TestMethod]
        public void MultithreadedTestRig_WaitForThreads()
        {
            var rig = new MultithreadedTestRig<TestContext>();

            foreach (var i in Enumerable.Range(0, 4))
                rig.CreateThread(
                    ctx =>
                    {
                        Thread.Sleep(250 * i);
                    },
                    ctx =>
                    {
                        ctx.CancellationRequested = true;
                    },
                    new TestContext());

            rig.CreateThread(ctx => { }, ctx => { }, new TestContext());

            var sw = new Stopwatch();

            sw.Start();
            
            rig.StartThreads();

            rig.WaitForThreads();

            sw.Stop();

            // 250 * i, when max(i) == 3
            sw.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(750); 
            rig.Threads.Select(t => t.Item1.IsAlive).Should().AllBeEquivalentTo(false);
            rig.Success.Should().BeTrue();
        }

        [TestMethod]
        public void MultithreadedTestRig_Exception()
        {
            var rig = new MultithreadedTestRig<TestContext>();

            rig.CreateThread(
                ctx =>
                {
                    while (true)
                    {
                        Thread.Sleep(100);
                        if (ctx.CancellationRequested)
                            break;
                    }
                },
                ctx =>
                {
                    ctx.CancellationRequested = true;
                },
                new TestContext()
                );

            rig.CreateThread(
                ctx =>
                {
                    Thread.Sleep(500);
                    throw new Exception("Exception message");
                },
                ctx =>
                {
                },
                new TestContext(), "Faulted thread");

            var sw = new Stopwatch();

            sw.Start();

            rig.StartThreads();
            rig.WaitForThreads();

            sw.Stop();

            sw.ElapsedMilliseconds.Should().BeGreaterOrEqualTo(500);

            rig.Threads.Select(t => t.Item1.IsAlive).Should().AllBeEquivalentTo(false);
            rig.Success.Should().BeFalse();
            rig.CaughtException.Should().NotBeNull();
            rig.CaughtException.Message.Should().Be("Exception message");
            rig.CaughtExceptionThread.Should().Be("Faulted thread");
        }

        [TestMethod]
        public void MultithreadedTestRig_ContractFailure()
        {
            var rig = new MultithreadedTestRig<TestContext>();

            rig.CreateThread(
                ctx =>
                {
                    Contract.Assert(false, "Contract failed");
                },
                ctx =>
                {
                    ctx.CancellationRequested = true;
                },
                new TestContext()
                );

            rig.StartThreads();
            rig.WaitForThreads();

            rig.Success.Should().BeFalse();
            rig.CaughtException.Should().NotBeNull();
        }
    }
}
