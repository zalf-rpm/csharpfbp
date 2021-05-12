using System;
using System.Collections.Generic;
using Capnp.Rpc;
using FBPLib;
using InfraCommon = Mas.Infrastructure.Common;
using Climate = Mas.Rpc.Climate;
using Soil = Mas.Rpc.Soil;
using Mgmt = Mas.Rpc.Management;
using Model = Mas.Rpc.Model;
using Common = Mas.Rpc.Common;

namespace Components
{
    [InPort("SR", description = "Sturdy reference to some capability")]
    [InPort("CT", description = "Name of Cap'n Proto interface this component knows returned by sturdy reference SR (default: Capnp.Rpc.BarProxy)")]
    [OutPort("OUT")]
    [ComponentDescription("Return a capability")]
    class ConnectToSturdyRef : Component
    {
        IInputPort _sturdyRefPort;
        string _sturdyRef;
        IInputPort _capTypePort;
        Type _capType = typeof(Capnp.Rpc.BareProxy);
        OutputPort _outPort;

        private InfraCommon.ConnectionManager ConMan() => (_network as CapabilityNetwork)?.ConMan;

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
            
            // read a string this component will convert to a type
            p = _capTypePort.Receive();
            if (p != null)
            {
                _capType = SupportedTypes.GetValueOrDefault(p.Content.ToString(), _capType);
                Drop(p);
                _capTypePort.Close();
            }

            try
            {
                if (ConMan() == null || _capType == null) return;

                dynamic task = typeof(InfraCommon.ConnectionManager)
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
            _outPort = OpenOutput("OUT");
        }

        public static Dictionary<string, Type> SupportedTypes = new()
        {
            { "TimeSeries", typeof(Climate.ITimeSeries) },
            { "EnvInstance<StructuredText,StructuredText>", typeof(Model.IEnvInstance<Common.StructuredText, Common.StructuredText>) },
            { "Capability", typeof(Capnp.Rpc.BareProxy) }
        };
    }
}
