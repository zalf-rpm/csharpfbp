using System;
using System.Linq;
using Climate = Mas.Rpc.Climate;
using Soil = Mas.Rpc.Soil;
using Mgmt = Mas.Rpc.Management;
using Model = Mas.Rpc.Model;
using ST = Mas.Rpc.Common.StructuredText;
using Capnp.Rpc;
using FBPLib;
using System.Collections.Generic;

namespace Components
{
    [InPort("IN", description = "Input containing all necessary data as attributes")]
    [OutPort("OUT")]
    [ComponentDescription("Create Model.Env<StructuredText> out of IPs received at IN.")]
    class CreateModelEnv2 : Component
    {
        IInputPort _inPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p;
            while ((p = _inPort.Receive()) != null)
            {
                if (p.Attributes.ContainsKey("rest") && p.Attributes["rest"] is ST rst)
                {
                    Model.Env<ST> env = new() { Rest = rst };

                    if (p.Attributes.ContainsKey("time-series") && p.Attributes["time-series"] is Climate.ITimeSeries ts)
                        env.TimeSeries = ts;

                    if (p.Attributes.ContainsKey("soil-profile") && p.Attributes["soil-profile"] is Soil.Profile sp)
                        env.SoilProfile = sp;

                    if (p.Attributes.ContainsKey("mgmt-events") && p.Attributes["mgmt-events"] is IEnumerable<Mgmt.Event> mes)
                        env.MgmtEvents = mes.ToList();
                    
                    var p2 = Create(env);
                    _outPort.Send(p2);
                }
                Drop(p);
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }
    }
}
