using System;
using System.Linq;
using Mas.Rpc;
using Capnp.Rpc;
using FBPLib;

namespace Components
{
    [InPort("CAP")]
    [OutPort("OUT")]
    [ComponentDescription("Receive capability and call it sending the result")]
    class CapabilityUse : Component
    {
        IInputPort _inPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _inPort.Receive();
            Mas.Rpc.Climate.ITimeSeries timeSeries = null;
            if (p != null)
            {
                timeSeries = p.Content as Mas.Rpc.Climate.ITimeSeries;
                Drop(p);
                _inPort.Close();
            }

            if (timeSeries != null)
            {
                try
                {
                    var header = timeSeries.Header().Result;
                    var hl = header.Select(h => h.ToString()).ToList();
                    p = Create(hl);
                    _outPort.Send(p);
                    timeSeries.Dispose();
                }
                catch (RpcException e) { Console.WriteLine(e.Message); }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("CAP");
            _outPort = OpenOutput("OUT");
        }

        public override void Dispose()
        {
        }
    }
}
