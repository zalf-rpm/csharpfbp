using FBPLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Components
{
    [InPort("IN", description = "input string to be split", type = typeof(string))]
    [InPort("CONF", description = "Configuration of the component. " +
        "split-at: (char ','=default) -> token to use for splitting, " +
        "add-brackets: (true | false=default) -> add substream brackets, " +
        "trim: (true=default | false) -> trim split strings")]
    [OutPort("OUT", type = typeof(string))]
    [ComponentDescription("Split string arriving at IN at tokens AT and send split parts to OUT")]
    class SplitString : Component
    {
        IInputPort _inPort;
        IInputPort _confPort;
        string _at = ",";
        bool _addSSBrackets = false;
        bool _trim = true;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _confPort.Receive();
            if (p != null)
            {
                try
                {
                    var conf = JObject.Parse(p.Content?.ToString() ?? "{}");
                    _at = conf["split-at"]?.Value<string>() ?? _at;
                    _addSSBrackets = conf["add-brackets"]?.Value<bool>() ?? _addSSBrackets;
                    _trim = conf["trim"]?.Value<bool>() ?? _trim;
                }
                catch (JsonReaderException) { }
                Drop(p);
                _confPort.Close();
            }

            while ((p = _inPort.Receive()) != null)
            {
                var str = p.Content.ToString();
                Drop(p);
                var strs = str.Split(_at).Select(s => _trim ? s.Trim() : s);
                if (strs.Count() > 1 && _addSSBrackets) _outPort.Send(Create(Packet.Types.Open, ""));
                foreach(var s in strs) _outPort.Send(Create(s));
                if (strs.Count() > 1 && _addSSBrackets) _outPort.Send(Create(Packet.Types.Close, ""));
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _confPort = OpenInput("CONF");
            _outPort = OpenOutput("OUT");
        }
    }
}
