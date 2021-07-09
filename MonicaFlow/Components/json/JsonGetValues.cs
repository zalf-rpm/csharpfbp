using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using FBPLib;
using ST = Mas.Rpc.Common.StructuredText;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace Components
{
    [InPort("IN", description = "JSON IP")]
    [InPort("CONF", description = "Configuration of the component. " +
        "The keys are integer numbers specifying the array port the selected value should be send on. " +
        "The values can either be an JSON object of with the following keys or a string which will denote a single key used for selection. " +
        "Subobject keys: " +
        "key | path -> select the subelement at given key or path," +
        "split (true | false=default) -> split selected subelement if it is a collection")]
    [OutPort("OUT", arrayPort = true)]
    [OutPort("PASS", optional = true)]
    [ComponentDescription("Select a multiple JSON values out of a JSON object and send it on selected array output ports.")]
    class JsonGetValues : Component
    {
        IInputPort _inPort;
        IInputPort _confPort;
        Dictionary<int, Tuple<List<string>, bool>> _conf = new();
        OutputPort[] _outPort;
        OutputPort _passPort;

        public override void Execute()
        {
            
            Packet p = _confPort.Receive();
            if (p != null)
            {
                try
                {
                    var conf = JObject.Parse(p.Content?.ToString() ?? "{}");
                    foreach(var (key, val) in conf)
                    {
                        if (int.TryParse(key, out int ikey))
                        {
                            switch(val.Type)
                            {
                                case JTokenType.String:
                                    _conf[ikey] = Tuple.Create(new List<string> { val.ToString() }, false);
                                    break;
                                case JTokenType.Array:
                                    _conf[ikey] = Tuple.Create(val.Select(k => k.Value<string>()).ToList(), false);
                                    break;
                                case JTokenType.Object:
                                    var split = conf.Value<bool>("split");
                                    var keys = conf.Value<JArray>("path")?.Select(k => k.Value<string>()).ToList() ?? new List<string>();
                                    var k = conf.Value<string>("key");
                                    if (k != null) keys = new() { k };
                                    _conf[ikey] = Tuple.Create(keys, split);
                                    break;
                            }
                        }
                    }
                }
                catch (JsonReaderException) { }
                Drop(p);
                _confPort.Close();
            }

            int portCount = _outPort.Length;

            if ((p = _inPort.Receive()) != null)
            {
                if(p.Content is JToken ojt)
                {
                    foreach (var (portNo, conf) in _conf)
                    {
                        var jt = ojt;

                        if (portNo >= portCount) continue;

                        var keys = conf.Item1;
                        var split = conf.Item2;

                        bool error = false;
                        foreach (var key in keys)
                        {
                            if (jt is JObject jo && jo.ContainsKey(key)) jt = jt[key];
                            else if (UInt64.TryParse(key, out UInt64 i) && jt is JArray ja) jt = ja[i];
                            else
                            {
                                error = true;
                                break;
                            }
                        }

                        if (split)
                        {
                            if (jt is JArray ja) foreach (var e in ja) _outPort[portNo].Send(Create(e));
                            else if (jt is JObject jo) foreach (var (k, v) in jo) _outPort[portNo].Send(Create(ValueTuple.Create(k, v)));
                        }
                        else if (!error) _outPort[portNo].Send(Create(jt));
                    }
                }
                if (_passPort.IsConnected()) _passPort.Send(p);
                else Drop(p);
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _confPort = OpenInput("CONF");
            _outPort = OpenOutputArray("OUT");
            _passPort = OpenOutput("PASS");
        }
    }
}
