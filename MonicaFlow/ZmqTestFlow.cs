using System;
using FBPLib;
using Components;
using System.Threading.Tasks;

namespace MonicaFlow
{ 
    public class ZmqTestFlow : CapabilityNetwork
    {
        string description = "Click anywhere on selection area";
        public override void Define()
        {
            Component("Connect_to_proxy", typeof(ZmqConsumer));
            Component("write_console", typeof(WriteToConsole));
            Connect(Component("Connect_to_proxy"), Port("OUT"), Component("write_console"), Port("IN"));
            Initialize("tcp://login01.cluster.zalf.de:7777", Component("Connect_to_proxy"), Port("CON"));
        }

        //static async Task Main(String[] argv)
        //{
        //    //AppContext.SetSwitch("Tracing", true);
         //   using var network = new ZmqTestFlow();
         //   await network.GoAsync();
         //   Console.WriteLine("bla");
        //}
    }
}
