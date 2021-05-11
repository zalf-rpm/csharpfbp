using System;
using Capnp.Rpc;
using FBPLib;
using Common = Mas.Infrastructure.Common;

namespace Components
{
    [InPort("SR", description = "Sturdy reference to some capability")]
    [InPort("CT", description = "Full dotnet name of Cap'n Proto interface returned by sturdy reference SR (default: Capnp.Rpc.BarProxy)")]
    [InPort("AFN", description = "Assembly full name, which contains the capability type CT")]
    [OutPort("CAP")]
    [ComponentDescription("Return a capability")]
    class ConnectToSturdyRef : Component, IDisposable
    {
        IInputPort _sturdyRefPort;
        string _sturdyRef;
        IInputPort _capTypePort;
        IInputPort _assemblyFullNamePort;
        string _assemblyFullName = "capnproto_schemas_csharp, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        Type _capType = typeof(Capnp.Rpc.BareProxy);
        OutputPort _outPort;

        private Common.ConnectionManager ConMan() => (_network as CapabilityNetwork)?.ConMan;

        public override void Execute()
        {
            // read mandatory sturdy ref to connect to
            Packet p = _sturdyRefPort.Receive();
            if (p == null) return;
            else
            {
                _sturdyRef = p.Content.ToString();
                Drop(p);
                _sturdyRefPort.Close();
            }

            // read assembly name IIP if there
            p = _assemblyFullNamePort.Receive();
            if (p != null)
            {
                _assemblyFullName = p.Content.ToString();
                Drop(p);
                _assemblyFullNamePort.Close();
            }

            // 
            p = _capTypePort.Receive();
            if (p != null)
            {
                _capType = Type.GetType($"{p.Content}, {_assemblyFullName}");
                Drop(p);
                _capTypePort.Close();
            }

            try
            {
                if (ConMan() == null || _capType == null) return;

                dynamic task = typeof(Common.ConnectionManager)
                    .GetMethod("Connect")
                    .MakeGenericMethod(_capType)
                    .Invoke(ConMan(), new object[] { _sturdyRef });
                var cap = task.Result;
                p = Create(cap);
                _outPort.Send(p);
            }
            catch (RpcException e) { Console.WriteLine(e.Message); }
        }

        public override void OpenPorts()
        {
            _sturdyRefPort = OpenInput("SR");
            _capTypePort = OpenInput("CT");
            _assemblyFullNamePort = OpenInput("AFN");
            _outPort = OpenOutput("CAP");
        }

        public void Dispose()
        {
        }
    }
}
