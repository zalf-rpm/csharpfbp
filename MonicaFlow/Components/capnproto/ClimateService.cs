using System;
using System.Linq;
using Capnp.Rpc;
using FBPLib;
using ST = Mas.Rpc.Common.StructuredText;
using Model = Mas.Rpc.Model;

namespace Components
{
    [InPort("CAP", description = "Capability")]
    [InPort("SR", description = "Sturdy reference")]
    [InPort("MSG", description = "")]
    [OutPort("OUT")]
    [ComponentDescription("Send ")]
    class ClimateService : Component
    {
        IInputPort _capPort;
        Proxy _cap = null;
        IInputPort _msgPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _capPort.Receive();
            if (p != null)
            {
                _cap = p.Content as Proxy;
                Drop(p);
            }

            p = _msgPort.Receive();
            if (p != null)
            {
                var msgName = p.Content.ToString();
                Drop(p);

                var t = _cap.GetType();
                var m = t.GetMethod(msgName);
                var ps = m.GetParameters();

                dynamic r = m.Invoke(_cap, new object[]{ null });
                var res = r.Result;

                try
                {
                    //var res = _cap.Run(env).Result;
                    //var jstr = res.Value.ToString();
                    //p = Create(res);// jstr);
                    //_outPort.Send(p);
                }
                catch (RpcException e) { Console.WriteLine(e.Message); }


                /*
                if (_cap != null && pc is Model.Env<ST> env)
                {
                    try
                    {
                        var res = _cap.Run(env).Result;
                        //var jstr = res.Value.ToString();
                        p = Create(res);// jstr);
                        _outPort.Send(p);
                    }
                    catch (RpcException e) { Console.WriteLine(e.Message); }
                }
                */
            }
        }

        public override void OpenPorts()
        {
            _capPort = OpenInput("CAP");
            _msgPort = OpenInput("MSG");
            _outPort = OpenOutput("OUT");
        }

        public override void Dispose()
        {
            _cap?.Dispose();
        }
    }
}
