using System;
using FBPLib;

namespace Components
{
    public abstract class CapabilityNetwork : Network, ICapabilityNetwork//, IDisposable
    {
        private Mas.Infrastructure.Common.ConnectionManager _conMan = new();

        public virtual Mas.Infrastructure.Common.ConnectionManager ConnectionManager()
        {
            return _conMan;
        }

        public override void Dispose()
        {
            _conMan?.Dispose();
        }
    }
}
