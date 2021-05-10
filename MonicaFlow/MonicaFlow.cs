using System;
using FBPLib;
using Components;
using System.Threading.Tasks;

namespace MonicaFlow
{ 
    public class MonicaFlow : CapabilityNetwork
    {
        string description = "Click anywhere on selection area";
        public override void Define()
        {
            Component("CapSrc", typeof(CapabilitySource));
            Component("CapUse", typeof(CapabilityUse));
            Component("WriteConsole", typeof(WriteToConsole));
            Initialize("capnp://localhost:11002", Component("CapSrc"), Port("SR"));
            Connect(Component("CapSrc"), Port("OUT"), Component("CapUse"), Port("IN"));
            Connect(Component("CapUse"), Port("OUT"), Component("WriteConsole"), Port("IN"));
            Initialize("Mas.Rpc.Climate.ITimeSeries", Component("CapSrc"), Port("CAPTYPE"));
        }

        static async Task Main(String[] argv)
        {
            using var network = new MonicaFlow();
            await network.GoA();
            Console.WriteLine("bla");
        }
    }
}
