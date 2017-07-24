using System;

namespace SharpToolkit.Extensions
{
    /// <summary>
    /// A base class for quckly implementing IDisposable when no other base class required.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        private bool disposed = false;
        private bool throwOnFinalize;

        /// <summary>
        /// Initializes new instance of Disposable, that doesn't throw on finalize.
        /// </summary>
        public Disposable() { }

        /// <summary>
        /// Can be overriden to track finalizer call, in cases when such call is considered a bug.
        /// </summary>
        protected virtual void ReportFinalization() { }

        /// <summary>
        /// This methods does the actual disposing.
        /// </summary>
        protected abstract void DisposeImpl();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                DisposeImpl();
            }
            
            disposed = true;
        }

        ~Disposable()
        {
            ReportFinalization();
            Dispose(false);
        }
    }
}
