using System;
using FBPLib;

namespace Components
{
    public abstract class CapabilitySubNet : SubNet, ICapabilityNetwork
    {
        public virtual Mas.Infrastructure.Common.ConnectionManager ConnectionManager()
        {
            var conMan = (_network as ICapabilityNetwork)?.ConnectionManager();
            if (conMan == null) Console.WriteLine("Error: Network is not CapabilityNetwork and doesn't support ConnectionManager.");
            return conMan;
        }
    }
}
