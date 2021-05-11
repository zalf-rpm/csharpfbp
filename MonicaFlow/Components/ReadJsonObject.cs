using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using FBPLib;

namespace Components
{
    [InPort("IN", description = "Path to a JSON file")]
    [OutPort("OUT", type = typeof(JObject))]
    [ComponentDescription("Read give FILE and parse as JSON object")]
    class ReadJsonObject : Component
    {
        IInputPort _inPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _inPort.Receive();
            if (p != null)
            {
                var filepath = p.Content.ToString();
                Drop(p);
                var fileContent = File.ReadAllText(filepath);
                var jobj = JObject.Parse(fileContent);
                p = Create(jobj);
                _outPort.Send(p);
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }
    }
}
