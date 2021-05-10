using System;
using FBPLib;

namespace Components
{
    public abstract class CapabilityNetwork : Network, IDisposable
    {
        public Mas.Infrastructure.Common.ConnectionManager ConMan = new();

        // To detect redundant calls
        private bool _disposed = false;

        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) ConMan?.Dispose();
            _disposed = true;
        }
    }
}
