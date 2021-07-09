using System;
using System.Linq;
using Capnp.Rpc;
using FBPLib;
using ST = Mas.Rpc.Common.StructuredText;
using Model = Mas.Rpc.Model;
using System.Reflection;

namespace Components
{
    [InPort("IN", description = "Capability")]
    [InPort("MSG", description = "RPC message to CAP")]
    [InPort("PARAMS", arrayPort = true, description = "Parameters to MSG")]
    [OutPort("OUT")]
    [ComponentDescription("Run the model given capability CAP and environment ENV, returning sending the result on OUT")]
    class SendMessage : Component
    {
        IInputPort _inPort;
        Proxy _cap = null;
        IInputPort _msgPort;
        MethodInfo _method;
        IInputPort[] _paramsPort;
        object[] _params;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _inPort.Receive();
            if (p != null)
            {
                _cap = p.Content as Proxy;
                Drop(p);
            }

            // we need a capability in order to continue
            if (_cap == null) return;

            p = _msgPort.Receive();
            if (p != null)
            {
                var msgName = p.Content.ToString();
                var t = _cap.GetType();
                _method = t.GetMethod(msgName);
                Drop(p);
            }

            // we need a method to continue
            if (_method == null) return;

            var no = _paramsPort.Length;
            _params = new object[no + 1];
            _params[no] = null;
            for (var i = 0; i < no; i++)
            {
                p = _paramsPort[i].Receive();
                if (p != null) 
                {
                    _params[i] = p.Content;
                    Drop(p);
                }
            }

            try
            {
                dynamic r = _method.Invoke(_cap, _params);
                _outPort.Send(Create(r.Result));
            }
            catch (RpcException e) 
            { 
                Console.WriteLine(e.Message); 
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _msgPort = OpenInput("MSG");
            _paramsPort = OpenInputArray("PARAMS");
            _outPort = OpenOutput("OUT");
        }

        public override void Dispose()
        {
            _cap?.Dispose();
        }
    }
}
