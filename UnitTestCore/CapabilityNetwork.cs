using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBPLib;
using Components;
using NUnit.Framework;

namespace UnitTests
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
