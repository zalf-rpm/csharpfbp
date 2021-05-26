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
    public class Class3
    {
        public static T Cast<T>(object o)
        {
            return (T)o;
        }
    }

    [InPort("IN", description = "Input containing all necessary data as attributes")]
    [OutPort("OUT")]
    [ComponentDescription("Receive capability and call it sending the result")]
    class CreateModelEnv2 : Component
    {
        IInputPort _inPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Climate.ITimeSeries timeSeries = null;
            Soil.Profile soilProfile = null;
            IEnumerable<Mgmt.Event> mgmtEvents = null;
            dynamic env = null;

            Packet p;
            while ((p = _inPort.Receive()) != null)
            {
             
                
                
                
                if (p.Type == Packet.Types.Open) _restLevel++;
                else if (p.Type == Packet.Types.Close) _restLevel--;
                else
                {
                    object rest = p.Content;
                    if (rest != null)
                    {
                        var restType = rest.GetType();
                        var envType = typeof(Model.Env<>).MakeGenericType(restType);
                        if (env = Activator.CreateInstance(envType) != null)
                        {
                            if (rest is ST rst) env.Rest = rst;
                        }
                        else return; // we need a valid env for everything else,
                    }

                }
                Drop(p);
            }

            p = null;
            if (timeSeries == null || _timeSeriesLevel > 0) p = _timeSeriesPort.Receive();
            if (p != null)
            {
                if (p.Type == Packet.Types.Open) _timeSeriesLevel++;
                else if (p.Type == Packet.Types.Close) _timeSeriesLevel--;
                else if (p.Content is Climate.ITimeSeries ts) env.TimeSeries = timeSeries = ts;
                Drop(p);
            }

            p = null;
            if (soilProfile == null || _soilProfileLevel > 0) p = _soilProfilePort.Receive();
            if (p != null)
            {
                if (p.Type == Packet.Types.Open) _soilProfileLevel++;
                else if (p.Type == Packet.Types.Close) _soilProfileLevel--;
                else if (p.Content is Soil.Profile sp) env.SoilProfile = soilProfile = sp;
                Drop(p);
            }

            p = null;
            if (mgmtEvents == null || _mgmtEventsLevel > 0) p = _mgmtEventsPort.Receive();
            if (p != null)
            {
                if (p.Type == Packet.Types.Open) _mgmtEventsLevel++;
                else if (p.Type == Packet.Types.Close) _mgmtEventsLevel--;
                else if (p.Content is IEnumerable<Mgmt.Event> es) env.cropRotation = mgmtEvents = es;
                Drop(p);
            }

            p = Create(env);
            _outPort.Send(p);
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }
    }
}
