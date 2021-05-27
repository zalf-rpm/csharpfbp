using Newtonsoft.Json.Linq;
using System.Linq;
using FBPLib;
using System.Collections.Generic;

namespace Components
{
    [InPort("IN", description = "Values to set in JSON object", type = typeof(string))]
    [InPort("CONF", description = "Configuration as JSON string", type = typeof(string))]
    [InPort("OBJ", description = "JSON object to be used as template (deep copy on every received IN) or forwarded", type = typeof(JObject))]
    [OutPort("OUT", type = typeof(JObject))]
    [ComponentDescription("Set value from IN on KEY in possibly deep cloned template object JOBJ")]
    class SetValueOnJsonTemplate : Component
    {
        IInputPort _inPort;
        IInputPort _confPort;
        string _sep = "/";
        List<string> _keys = new();
        bool _clone = true;
        bool _create = false;
        IInputPort _objPort; 
        JObject _obj = null;
        OutputPort _outPort;

        public override void Execute()
        {
            // read CONF IIP
            Packet p = _confPort.Receive();
            if (p != null)
            {
                var conf = JObject.Parse(p.Content?.ToString() ?? "{}");
                Drop(p);
                _sep = conf.Value<string>("sep") ?? _sep;
                _clone = conf.Value<bool>("clone");
                _create = conf.Value<bool>("create");
                _keys = conf.Value<JArray>("path")?.Select(k => k.Value<string>()).ToList() ?? _keys;
                var k = conf.Value<string>("key");
                if (k != null) _keys = new() { k };
                _confPort.Close();
            }

            p = _objPort.Receive();
            if (p != null)
            {
                if (p.Content is JObject jobj) _obj = jobj;
                Drop(p);
                // if we got no template we can close down the component and possibly the whole graph
                if(_obj == null)
                {
                    _inPort.Close();
                    _objPort.Close();
                    _outPort.Close();
                }
            }

            p = _inPort.Receive();
            if (p != null)
            {
                var str = p.Content.ToString();
                Drop(p);
                var t = _clone ? _obj.DeepClone() : _obj;
                if (str != null && _keys.Any())
                {
                    var subt = t;
                    foreach(var (i, key) in _keys.Select((k,i) => (i,k)))
                    {
                        var last = i == _keys.Count - 1;
                        if (subt.Contains(key) || last)
                        {
                            if (last) subt[key] = str;
                            else subt = subt[key];
                        }
                        else break;
                    }
                }
                p = Create(t);
                _outPort.Send(p);
                // if we sent _jobj, we have to read another one first
                if (!_clone) _obj = null;
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _confPort = OpenInput("CONF");
            _objPort = OpenInput("OBJ");
            _outPort = OpenOutput("OUT");
        }
    }
}
