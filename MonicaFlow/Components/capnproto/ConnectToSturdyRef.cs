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
    [InPort("IN", description = "Sturdy reference to some capability")]
    [InPort("TYPE", description = "Name of Cap'n Proto interface this component knows returned by sturdy reference SR (default: Capnp.Rpc.BarProxy)")]
    [OutPort("OUT")]
    [ComponentDescription("Return a capability")]
    class ConnectToSturdyRef : Component
    {
        IInputPort _sturdyRefPort;
        string _sturdyRef;
        IInputPort _capTypePort;
        //Type _capType = typeof(Capnp.Rpc.BareProxy);
        Type _capType = typeof(Proxy);
        OutputPort _outPort;

        private InfraCommon.ConnectionManager _conMan = new();

        //private InfraCommon.ConnectionManager ConMan() => (_network as ICapabilityNetwork)?.ConnectionManager();

        public override void Execute()
        {
            /*
            if (ConMan() == null)
            {
                Console.WriteLine("No ConnectionManager instance available. Closing down process.");
                _sturdyRefPort.Close();
                _capTypePort.Close();
                return;
            }
            //*/

            // read mandatory sturdy ref to connect to
            Packet p = _sturdyRefPort.Receive();
            if (p != null)
            {
                _sturdyRef = p.Content.ToString();
                Drop(p);
                _sturdyRefPort.Close();
            }

            if (string.IsNullOrEmpty(_sturdyRef))
            {
                Console.WriteLine("Received sturdy ref is invalid or empty. Closing down process because it is needed.");
                _capTypePort.Close();
                return;
            }

            // read a string this component will convert to a type
            p = _capTypePort.Receive();
            if (p != null)
            {
                _capType = SupportedTypes.GetValueOrDefault(p.Content.ToString(), _capType);
                Drop(p);
                _capTypePort.Close();

                if(_capType == null)
                {
                    Console.WriteLine("Received capability type on TYPE port is unknown. Closing down process because it is needed.");
                    return;
                }
            }

            try
            {
                dynamic task = typeof(InfraCommon.ConnectionManager)
                    .GetMethod("Connect")
                    .MakeGenericMethod(_capType)
                    .Invoke(_conMan, new object[] { _sturdyRef });
                    //.Invoke(ConMan(), new object[] { _sturdyRef });
                _outPort.Send(Create(task.Result));
            }
            catch (RpcException e) { Console.WriteLine(e.Message); }
        }

        public override void OpenPorts()
        {
            _sturdyRefPort = OpenInput("IN");
            _capTypePort = OpenInput("TYPE");
            _outPort = OpenOutput("OUT");
        }

        public static Dictionary<string, Type> SupportedTypes = new()
        {
            { "ClimateService", typeof(Climate.IService) },
            { "ClimateDataset", typeof(Climate.IDataset) },
            { "TimeSeries", typeof(Climate.ITimeSeries) },
            { "SoilService", typeof(Soil.IService) },
            { "EnvInstance<StructuredText,StructuredText>", typeof(Model.IEnvInstance<Common.StructuredText, Common.StructuredText>) },
            { "Capability", typeof(Capnp.Rpc.BareProxy) }
        };
        
        public override void Dispose()
        {
            _conMan?.Dispose();
        }
    }
}
