using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;

namespace SharpToolkit.Extensions.Diagnostics.Testing
{
    public class MultithreadedTestRig
    {
        public static Action OnContractFailedAction { get; set; }

        public class ContractFailedOnTestException : Exception
        {
            public string Condition { get; }

            public ContractFailedOnTestException(string message, string condition, Exception originalException) : base(message, originalException)
            {
                this.Condition = condition;
            }
        }

        static MultithreadedTestRig()
        {
            Contract.ContractFailed += (sender, args) =>
            {
                if (OnContractFailedAction != null)
                    OnContractFailedAction();

                throw new ContractFailedOnTestException(args.Message, args.Condition, args.OriginalException);
            };
        }
    }

    public class MultithreadedTestRig<TThreadContext> : MultithreadedTestRig
    {
        private AutoResetEvent[] mainContinueEvent;

        private object syncRoot = 
            new object();

        private List<(Thread, TThreadContext, Action<TThreadContext>)> threads;
        public IReadOnlyList<(Thread, TThreadContext, Action<TThreadContext>)> Threads => threads;

        public bool Success { get; private set; } = true;
        public Exception CaughtException { get; private set; }
        public string CaughtExceptionThread { get; private set; }

        public MultithreadedTestRig()
        {
            this.threads = new List<(Thread, TThreadContext, Action<TThreadContext>)>();
        }

        public void CreateThread(Action<TThreadContext> runAct, Action<TThreadContext> abortAct, TThreadContext context)
        {
            CreateThread(runAct, abortAct, context, "TestThread");
        }

        public void CreateThread(Action<TThreadContext> runAct, Action<TThreadContext>abortAct, TThreadContext context, string name)
        {
            var t = new Thread((object i) =>
            {
                try
                {
                    runAct(context);
                }
                catch (Exception e)
                {
                    lock (this.syncRoot)
                    {
                        if (this.CaughtException == null)
                        {
                            this.CaughtException = e;
                            this.CaughtExceptionThread = Thread.CurrentThread.Name;
                            this.Success = false;

                            AbortThreads();
                        }
                    }
                }
                finally
                {
                    this.mainContinueEvent[(int)i].Set();
                }
            });

            t.Name = name;

            this.threads.Add((t, context, abortAct));
        }

        public void AbortThreads()
        {
            foreach (var (thread, ctx, abortAct) in this.Threads)
            {
                if (thread != Thread.CurrentThread)
                    abortAct(ctx);
            }
        }

        public void StartThreads()
        {
            this.mainContinueEvent =
                this.Threads.Select(t => new AutoResetEvent(false)).ToArray();

            for (int i = 0; i < this.Threads.Count; i += 1)
                this.Threads[i].Item1.Start(i);
        }

        public void WaitForThreads()
        {
            WaitHandle.WaitAll(this.mainContinueEvent);
        }
    }
}
