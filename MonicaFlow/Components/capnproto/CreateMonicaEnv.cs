﻿using Newtonsoft.Json.Linq;
using FBPLib;
using Monica;

namespace Components
{
    [InPort("SIM", description = "sim data", type = typeof(JObject))]
    [InPort("CROP", description = "crop data", type = typeof(JObject))]
    [InPort("SITE", description = "site data", type = typeof(JObject))]
    [OutPort("OUT")]
    [ComponentDescription("take SIM, CROP, SITE inputs and create JSON env for MONICA")]
    class CreateMonicaEnv : Component
    {
        IInputPort _simPort;
        IInputPort _cropPort;
        IInputPort _sitePort;
        OutputPort _outPort;
        RunMonica _runMonica = new(new MonicaIO());

        public override void Execute()
        {
            Packet p = _simPort.Receive();
            JObject simj = null;
            if (p != null)
            {
                simj = p.Content as JObject;
                Drop(p);
                if (simj == null) return;
            }

            p = _cropPort.Receive();
            JObject cropj = null;
            if (p != null)
            {
                cropj = p.Content as JObject;
                Drop(p);
                if (cropj == null) return;
            }

            p = _sitePort.Receive();
            JObject sitej = null;
            if (p != null)
            {
                sitej = p.Content as JObject;
                Drop(p);
                if (sitej == null) return;
            }

            var envj = _runMonica.CreateMonicaEnv(simj, cropj, sitej, null);
            p = Create(envj);
            _outPort.Send(p);
        }

        public override void OpenPorts()
        {
            _simPort = OpenInput("SIM");
            _cropPort = OpenInput("CROP");
            _sitePort = OpenInput("SITE");
            _outPort = OpenOutput("OUT");
        }
    }
}
