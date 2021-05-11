using Newtonsoft.Json.Linq;
using FBPLib;
using Monica;

namespace Components
{
    [InPort("SIM", description = "sim data", type = typeof(JObject))]
    [InPort("CROP", description = "crop data", type = typeof(JObject))]
    [InPort("SITE", description = "site data", type = typeof(JObject))]
    [OutPort("ENV")]
    [ComponentDescription("take SIM, CROP, SITE inputs and create JSON env for MONICA")]
    class CreateMonicaEnv : Component
    {
        IInputPort _simPort;
        IInputPort _cropPort;
        IInputPort _sitePort;
        OutputPort _envPort;
        RunMonica _runMonica = new(new MonicaIO());

        public override void Execute()
        {
            Packet p = _simPort.Receive();
            JObject simj = null;
            if (p != null)
            {
                simj = p.Content as JObject;
                if (simj == null) return;
                Drop(p);
            }

            p = _cropPort.Receive();
            JObject cropj = null;
            if (p != null)
            {
                cropj = p.Content as JObject;
                if (cropj == null) return;
                Drop(p);
            }

            p = _sitePort.Receive();
            JObject sitej = null;
            if (p != null)
            {
                sitej = p.Content as JObject;
                if (sitej == null) return;
                Drop(p);
            }

            var envj = _runMonica.CreateMonicaEnv(simj, cropj, sitej, null);
            p = Create(envj);
            _envPort.Send(p);
        }

        public override void OpenPorts()
        {
            _simPort = OpenInput("SIM");
            _cropPort = OpenInput("CROP");
            _sitePort = OpenInput("SITE");
            _envPort = OpenOutput("ENV");
        }
    }
}
