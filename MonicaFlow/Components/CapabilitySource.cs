using System;
using Mas.Rpc;
using Capnp.Rpc;
using FBPLib;
using Common = Mas.Infrastructure.Common;
using System.Threading.Tasks;

namespace Components
{
    [InPort("SR")]
    [InPort("CAPTYPE")]
    [OutPort("OUT")]
    [ComponentDescription("Return a capability")]
    class CapabilitySource : Component, IDisposable
    {
        IInputPort _sturdyRefPort;
        string _sturdyRef = "capnp://login01.cluster.zalf.de:11002";
        IInputPort _capTypePort;
        Type _capType;
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

            p = _capTypePort.Receive();
            if (p != null)
            {
                _capType = Type.GetType($"{p.Content}, capnproto_schemas_csharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
                Drop(p);
                _capTypePort.Close();
            }

            try
            {
                if (ConMan() == null || _capType == null) return;

                //dynamic task = typeof(Common.ConnectionManager)
                //    .GetMethod("Connect")
                //    .MakeGenericMethod(_capType)
                //    .Invoke(ConMan(), new object[] { _sturdyRef });
                //using var cap = task.Result;
                var cap = ConMan().Connect<Mas.Rpc.Climate.ITimeSeries>(_sturdyRef).Result;
                //p = Create(Capnp.Rpc.Proxy.Share(cap));
                p = Create(cap);
                _outPort.Send(p);
            }
            catch (RpcException e) { Console.WriteLine(e.Message); }
        }

        public override void OpenPorts()
        {
            _sturdyRefPort = OpenInput("SR");
            _capTypePort = OpenInput("CAPTYPE");
            _outPort = OpenOutput("OUT");
        }

        public void Dispose()
        {
        }
    }
}
