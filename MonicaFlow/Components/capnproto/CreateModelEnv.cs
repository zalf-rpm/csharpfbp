using System;
using Climate = Mas.Rpc.Climate;
using Soil = Mas.Rpc.Soil;
using Mgmt = Mas.Rpc.Management;
using Model = Mas.Rpc.Model;
using ST = Mas.Rpc.Common.StructuredText;
using FBPLib;
using System.Collections.Generic;

namespace Components
{
    [InPort("REST", description = "RestInput")]
    [InPort("CLIM", description = "Mas.Rpc.Climate.ITimeSeries capability")]
    [InPort("SOIL", description = "Mas.Rpc.Soil.Profile structure")]
    [InPort("MGMT", description = "Enumerable of Mas.Rpc.Management.Event ")]
    [OutPort("OUT")]
    [ComponentDescription("For each received REST and potential CLIMate timeseries, SOILprofile and MGMTevents create an env for OUT")]
    class CreateModelEnv : Component
    {
        IInputPort _restPort;
        IInputPort _timeSeriesPort;
        IInputPort _soilProfilePort;
        IInputPort _mgmtEventsPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Climate.ITimeSeries timeSeries = null;
            bool disposeTS = true;
            Packet p = _timeSeriesPort.Receive();
            if (p != null)
            {
                timeSeries = p.Content as Climate.ITimeSeries;
                Drop(p);
            }

            IEnumerable<Mgmt.Event> mgmtEvents = null;
            p = _mgmtEventsPort.Receive();
            if (p != null)
            {
                mgmtEvents = p.Content as IEnumerable<Mgmt.Event>;
                Drop(p);
            }

            Soil.Profile soilProfile = null;
            p = _soilProfilePort.Receive();
            if (p != null)
            {
                soilProfile = p.Content as Soil.Profile;
                Drop(p);
            }
            
            p = _restPort.Receive();
            if (p != null)
            {
                object rest = p.Content;
                Drop(p);
                if (rest != null)
                {
                    var restType = rest.GetType();
                    var envType = typeof(Model.Env<>).MakeGenericType(restType);
                    dynamic env = Activator.CreateInstance(envType);
                    if (env != null)
                    {
                        disposeTS = false;
                        if (rest is ST rst) env.Rest = rst;
                        env.TimeSeries = timeSeries;
                        env.SoilProfile = soilProfile;
                        env.MgmtEvents = mgmtEvents;

                        _outPort.Send(Create(env));
                    }
                }
            }

            // if there was no REST or no env could be created dispose potential timeSeries cap
            if (disposeTS) timeSeries?.Dispose();
        }

        public override void OpenPorts()
        {
            _restPort = OpenInput("REST");
            _timeSeriesPort = OpenInput("CLIM");
            _soilProfilePort = OpenInput("SOIL");
            _mgmtEventsPort = OpenInput("MGMT");
            _outPort = OpenOutput("OUT");
        }
    }
}
