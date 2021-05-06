using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mas.Rpc;
using Capnp.Rpc;
using FBPLib;
using Common = Mas.Infrastructure.Common;

namespace UnitTests
{
    [InPort("SR")]
    [OutPort("OUT")]
    [ComponentDescription("Receive requests from capability")]
    class CapabilitySource : Component, IDisposable
    {
        IInputPort _sturdyRefPort;
        string _sturdyRef = "capnp://login01.cluster.zalf.de:11002";
        OutputPort _outPort;

        private Common.ConnectionManager ConMan() => (_network as CapabilityNetwork)?.ConMan;

        public override void Execute()
        {
            Packet p = _sturdyRefPort.Receive();
            if (p != null)
            {
                _sturdyRef = p.Content.ToString();
                Drop(p);
                _sturdyRefPort.Close();
            }

            try
            {
                if (ConMan() == null) return;

                using var timeSeries = ConMan().Connect<Mas.Rpc.Climate.ITimeSeries>(_sturdyRef).Result;
                //var header = timeSeries.Header().Result;
                //var hl = header.Select(h => h.ToString()).ToList();
                //Console.WriteLine(hl.Aggregate((a, v) => a + " | " + v));
                //p = Create(hl);
                p = Create(Capnp.Rpc.Proxy.Share(timeSeries));
                _outPort.Send(p);
            }
            catch (RpcException e) { Console.WriteLine(e.Message); }
        }

        public override void OpenPorts()
        {
            _sturdyRefPort = OpenInput("SR");
            _outPort = OpenOutput("OUT");
        }

        public void Dispose()
        {
        }
    }
}
