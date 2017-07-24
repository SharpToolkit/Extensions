using System;

namespace SharpToolkit.Extensions
{
    /// <summary>
    /// A base class for quckly implementing IDisposable when no other base class required.
    /// </summary>
    public abstract class Disposable : IDisposable
    {
        private bool disposed = false;
        
        /// <summary>
        /// Enables tracking of finalizer calls, in cases when those are considered bugs.
        /// </summary>
        protected virtual void ReportFinalization() { }

        /// <summary>
        /// Disposes of managed resources.
        /// </summary>
        protected abstract void DisposeManaged();

        /// <summary>
        /// Disposes of unmanaged resources.
        /// </summary>
        protected abstract void DisposeUnmanaged();
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                DisposeManaged();
            }

            DisposeUnmanaged();
            
            disposed = true;
        }

        ~Disposable()
        {
            ReportFinalization();
            Dispose(false);
        }
    }
}
