using System;
using System.Linq;
using Capnp.Rpc;
using FBPLib;
using ST = Mas.Rpc.Common.StructuredText;
using Model = Mas.Rpc.Model;

namespace Components
{
    [InPort("CAP", description = "EnvModel capability")]
    [InPort("ENV", description = "Environment to use for EnvModel run")]
    [OutPort("OUT")]
    [ComponentDescription("Run the model given capability CAP and environment ENV, returning sending the result on OUT")]
    class RunEnvModel : Component, IDisposable
    {
        IInputPort _capPort;
        Model.IEnvInstance<ST, ST> _cap;
        IInputPort _envPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _capPort.Receive();
            if (p != null)
            {
                _cap = p.Content as Model.IEnvInstance<ST, ST>;
                Drop(p);
                _capPort.Close();
            }

            p = _envPort.Receive();
            if (p != null)
            {
                var pc = p.Content;
                Drop(p);
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
