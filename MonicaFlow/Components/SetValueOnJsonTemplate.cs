using Newtonsoft.Json.Linq;
using System.Linq;
using FBPLib;
using System.Collections.Generic;

namespace Components
{
    [InPort("IN", description = "Values to set in JSON object", type = typeof(string))]
    [InPort("CONF", description = "Configuration as JSON string", type = typeof(string))]
    //[InPort("SEP", description = "Path separator token for composite key, is optional, default = '/'", type = typeof(string))]
    //[InPort("KEY", description = "Key to set in JSON object, is optional, if not connected will deep copy T for every packet on IN", type = typeof(string))]
    //[InPort("CREATE", description = "Create subobjects for path in KEY. Default = false", type = typeof(string))]
    [InPort("OBJ", description = "JSON object to be used as template (deep copy on every received IN) or forwarded", type = typeof(JObject))]
    //[InPort("CLONE", description = "Any received packet except null, false, no, will be considered true. Defaults to true.", type = typeof(JObject))]
    [OutPort("OUT", type = typeof(JObject))]
    [ComponentDescription("Set value from IN on KEY in possibly deep cloned template object JOBJ")]
    class SetValueOnJsonTemplate : Component
    {
        IInputPort _inPort;
        IInputPort _confPort;
        //IInputPort _sepPort;
        string _sep = "/";
        //IInputPort _keyPort;
        List<string> _keys = new();
        //IInputPort _clonePort;
        bool _clone = true;
        //IInputPort _createPort;
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

            /*
            // read SEP IIP
            Packet p = _sepPort.Receive();
            if (p != null)
            {
                _sep = p.Content?.ToString() ?? _sep;
                Drop(p);
                _sepPort.Close();
            }

            // read KEY IIP
            p = _keyPort.Receive();
            if (p != null)
            {
                _keys = p.Content.ToString().Split(_sep).Select(s => s.Trim()).ToList();
                Drop(p);
                _keyPort.Close();
            }

            // read CLONE IIP
            p = _clonePort.Receive();
            if (p != null)
            {
                var c = p.Content?.ToString().ToUpper();
                _clone = c != null && c != "FALSE" && c != "NO";
                Drop(p);
                _clonePort.Close();
            }

            // read CREATE IIP
            p = _createPort.Receive();
            if (p != null)
            {
                var c = p.Content?.ToString().ToUpper();
                _create = c != null && c != "FALSE" && c != "NO";
                Drop(p);
                _createPort.Close();
            }
            */

            p = _objPort.Receive();
            if (p != null)
            {
                if (p.Content is JObject jobj) _obj = jobj;
                Drop(p);
                //_jobjPort.Close();
                // if we got no template we can close down the component and possibly the whole graph
                if(_obj == null)
                {
                    _inPort.Close();
                    _sepPort.Close();
                    _keyPort.Close();
                    _clonePort.Close();
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
                    var last = false;
                    foreach(var (i, key) in _keys.Select((k,i) => (i,k)))
                    {
                        last = i == _keys.Count - 1;
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
            //_sepPort = OpenInput("SEP");
            //_keyPort = OpenInput("KEY");
            _objPort = OpenInput("OBJ");
            //_clonePort = OpenInput("CLONE");
            //_createPort = OpenInput("CREATE");
            _outPort = OpenOutput("OUT");
        }
    }
}
