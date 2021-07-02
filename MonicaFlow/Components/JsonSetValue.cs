using Newtonsoft.Json.Linq;
using System.Linq;
using FBPLib;
using System.Collections.Generic;

namespace Components
{
    [InPort("VAL", description = "Values to set in JSON object", type = typeof(string))]
    [InPort("KEY", description = "Key to be used for setting VAL. " +
        "If just a JSON string, will be taken as single key, if a JSON array as a path.", type = typeof(string))]
    [InPort("OBJ", description = "JSON object for VAL to be set on. If CONF.clone = true, will be deep cloned else forwarded", type = typeof(JObject))]
    [InPort("CONF", description = "Configuration as JSON object string. " +
        "clone: (true | false=default) -> if true, DeepClone OBJ on every set VAL = means OBJ acts as IIP", type = typeof(string))]
    [OutPort("OUT", type = typeof(JObject))]
    [ComponentDescription("Set value from IN on KEY in possibly deep cloned template object JOBJ")]
    class JsonSetValue : Component
    {
        IInputPort _valPort;
        IInputPort _keyPort;
        IInputPort _confPort;
        List<string> _keys = new();
        bool _clone = false;
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
                _clone = conf.Value<bool>("clone");
                _confPort.Close();
            }

            p = _keyPort.Receive();
            if (p != null)
            {
                var keyStr = p.Content?.ToString() ?? "";
                Drop(p);
                if (!keyStr.StartsWith('[')) keyStr = "\"" + keyStr + "\"";
                try
                {
                    var key = JToken.Parse(keyStr);
                    if (key is JArray ja) _keys = ja.Select(k => k.Value<string>()).ToList();
                    else if (key.Type == JTokenType.String) _keys = new() { key.ToString() };
                }
                catch (System.Exception) { }
                
                // if there is no key or path, close down the component
                if (!_keys.Any())
                {
                    _valPort.Close();
                    _objPort.Close();
                    _outPort.Close();
                }
                _keyPort.Close();
            }

            p = _objPort.Receive();
            if (p != null)
            {
                if (p.Content is JObject jobj) _obj = jobj;
                Drop(p);
            }

            // if we got no template we can close down the component and possibly the whole graph
            if (_obj == null)
            {
                _valPort.Close();
                _objPort.Close();
                _outPort.Close();
                _keyPort.Close();
            }

            p = _valPort.Receive();
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
                _outPort.Send(Create(t));
                // if we sent _jobj, we have to read another one first
                if (!_clone) _obj = null;
            }
        }

        public override void OpenPorts()
        {
            _valPort = OpenInput("VAL");
            _keyPort = OpenInput("KEY");
            _confPort = OpenInput("CONF");
            _objPort = OpenInput("OBJ");
            _outPort = OpenOutput("OUT");
        }
    }
}
