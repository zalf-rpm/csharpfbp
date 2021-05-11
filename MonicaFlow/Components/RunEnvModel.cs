using System;
using System.Linq;
using Capnp.Rpc;
using FBPLib;
using C = Mas.Rpc.Common;
using Model = Mas.Rpc.Model;

namespace Components
{
    [InPort("CAP", description = "EnvModel capability")]
    [InPort("ENV", description = "Environment to use for EnvModel run")]
    [OutPort("OUT")]
    [ComponentDescription("Receive capability and call it sending the result")]
    class RunEnvModel : Component, IDisposable
    {
        IInputPort _capPort;
        Model.IEnvInstance<C.StructuredText, C.StructuredText> _cap;
        IInputPort _envPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _capPort.Receive();
            if (p == null) return;
            else
            {
                _cap = p.Content as Model.IEnvInstance<C.StructuredText, C.StructuredText>;
                Drop(p);
                _capPort.Close();
            }

            p = _envPort.Receive();
            if (p != null)
            {
                if (_cap != null && p.Content is Model.Env<C.StructuredText> env)
                {
                    try
                    {
                        var res = _cap.Run(env).Result;
                        var jstr = res.Value.ToString();
                        p = Create(jstr);// res);
                        _outPort.Send(p);
                    }
                    catch (RpcException e) { Console.WriteLine(e.Message); }
                }
                Drop(p);
            }
        }

        public override void OpenPorts()
        {
            _capPort = OpenInput("CAP");
            _envPort = OpenInput("ENV");
            _outPort = OpenOutput("OUT");
        }

        public void Dispose()
        {
            _cap?.Dispose();
        }
    }
}
