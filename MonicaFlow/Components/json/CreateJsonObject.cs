using Newtonsoft.Json.Linq;
using FBPLib;

namespace Components
{
    [InPort("IN", description = "JSON string", type = typeof(string))]
    [OutPort("OUT", type = typeof(JObject))]
    [ComponentDescription("Parse IN string as JSON object")]
    class CreateJsonObject : Component
    {
        IInputPort _inPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _inPort.Receive();
            if (p != null)
            {
                var str = p.Content.ToString();
                Drop(p);
                if (str != null)
                {
                    var jobj = JObject.Parse(str);
                    p = Create(jobj);
                    _outPort.Send(p);
                }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }
    }
}
