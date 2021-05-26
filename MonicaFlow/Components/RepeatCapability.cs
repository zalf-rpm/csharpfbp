using System;
using System.Linq;
using Mas.Rpc;
using Capnp.Rpc;
using FBPLib;

namespace Components
{
    [InPort("IN", description = "Capability to repeat")]
    [InPort("COUNT", description = "How many times to repeat capability CAP")]
    [OutPort("OUT")]
    [ComponentDescription("Receive capability and copy it COUNT times to OUT")]
    class RepeatCapability : Component
    {
        IInputPort _inPort;
        IInputPort _countPort;
        ulong _count = 1;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _countPort.Receive();
            if (p != null)
            {
                _count = Math.Max(0, UInt64.Parse(p.Content.ToString()));
                Drop(p);
                _countPort.Close();
            }

            p = _inPort.Receive();
            if (p != null && p.Content is Capnp.Rpc.Proxy cap)
            {
                Drop(p);
                for(ulong i = 0; i < _count; i++) _outPort.Send(Create(Capnp.Rpc.Proxy.Share(cap)));
                cap.Dispose();
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _countPort = OpenInput("COUNT");
            _outPort = OpenOutput("OUT");
        }
    }
}
