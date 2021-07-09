using Newtonsoft.Json.Linq;
using FBPLib;
using System;

namespace Components
{
    [InPort("IN", description = "JSON string", type = typeof(string))]
    [InPort("TYPE", description = "What to create: Array | Object | Token = default", type = typeof(string))]
    [OutPort("OUT", type = typeof(JObject))]
    [ComponentDescription("Parse IN string as JSON object")]
    class JsonCreate : Component
    {
        enum JT { token, arr, obj };
        IInputPort _inPort;
        IInputPort _typePort;
        JT _type = JT.token;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _typePort.Receive();
            if (p != null)
            {
                var t = p.Content?.ToString();
                if (t != null)
                {
                    if (t.ToUpper() == "OBJECT") _type = JT.obj;
                    else if (t.ToUpper() == "ARRAY") _type = JT.arr;
                }
                Drop(p);
            }

            p = _inPort.Receive();
            if (p != null)
            {
                var str = p.Content.ToString();
                Drop(p);
                if (str != null)
                {
                    JToken jt;
                    try
                    {
                        switch (_type)
                        {
                            case JT.arr: jt = JArray.Parse(str); break;
                            case JT.obj: jt = JObject.Parse(str); break;
                            default: jt = JToken.Parse(str); break;
                        }
                        _outPort.Send(Create(jt));
                    }
                    catch (System.Exception e)
                    {
                        Console.WriteLine("Exception parsing JSON string. Exception: " + e.Message);
                    }
                }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _typePort = OpenInput("TYPE");
            _outPort = OpenOutput("OUT");
        }
    }
}
