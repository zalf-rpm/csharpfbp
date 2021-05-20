using System;
using FBPLib;

namespace Components
{
    public abstract class CapabilityNetwork : Network, IDisposable, ICapabilityNetwork
    {
        private Mas.Infrastructure.Common.ConnectionManager _conMan = new();

        public virtual Mas.Infrastructure.Common.ConnectionManager ConnectionManager()
        {
            return _conMan;
        }

        // To detect redundant calls
        private bool _disposed = false;

        public void Dispose() => Dispose(true);

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing) _conMan?.Dispose();
            _disposed = true;
        }
    }
}
