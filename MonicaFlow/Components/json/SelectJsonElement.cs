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
    [InPort("OPTS", description = "Options configuring the component, " +
        "key | path -> select the subelement at given key or path," +
        "split (true=default | false) -> split selected subelement if it is a collection")]
    [OutPort("OUT")]
    [OutPort("PASS", optional = true)]
    [ComponentDescription("Select a JSON element and output it.")]
    class SelectJsonElement : Component
    {
        IInputPort _inPort;
        IInputPort _confPort;
        List<string> _keys = new();
        bool _split = false;
        OutputPort _outPort;
        OutputPort _passPort;

        public override void Execute()
        {
            Packet p = _confPort.Receive();
            if (p != null)
            {
                try
                {
                    var conf = JObject.Parse(p.Content?.ToString() ?? "{}");
                    _split = conf.Value<bool>("split");
                    _keys = conf.Value<JArray>("path")?.Select(k => k.Value<string>()).ToList() ?? _keys;
                    var k = conf.Value<string>("key");
                    if (k != null) _keys = new() { k };
                }
                catch (JsonReaderException) { }
                Drop(p);
                _confPort.Close();
            }

            if ((p = _inPort.Receive()) != null)
            {
                if(p.Content is JToken jt)
                {
                    bool error = false;
                    foreach (var key in _keys)
                    {
                        UInt64 i;
                        if (jt is JObject jo && jo.ContainsKey(key)) jt = jt[key];
                        else if (UInt64.TryParse(key, out i) && jt is JArray ja) jt = ja[i];
                        else
                        {
                            error = true;
                            break;
                        }
                    }

                    if(_split)
                    {
                        if (jt is JArray ja) foreach (var e in ja) _outPort.Send(Create(e));
                        else if (jt is JObject jo) foreach (var (k, v) in jo) _outPort.Send(Create(ValueTuple.Create(k, v)));
                    } 
                    else if(!error) _outPort.Send(Create(jt));
                }
                if (_passPort.IsConnected()) _passPort.Send(p);
                else Drop(p);
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _confPort = OpenInput("CONF");
            _outPort = OpenOutput("OUT");
            _passPort = OpenOutput("PASS");
        }
    }
}
