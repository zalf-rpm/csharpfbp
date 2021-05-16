using System;
using System.Linq;
using Mas.Rpc;
using Capnp.Rpc;
using FBPLib;
using ST = Mas.Rpc.Common.StructuredText;

namespace Components
{
    [InPort("IN", description = "String representing the structured text")]
    [InPort("STR", description = "one of [JSON, XML, none] - names are case-insensitive")]
    [OutPort("OUT")]
    [ComponentDescription("Create StructuredText out of content at IN and structure type at STR")]
    class CreateStructuredText : Component
    {
        IInputPort _inPort;
        IInputPort _strPort;
        ST.structure _structure = new ST.structure { which = ST.structure.WHICH.None };
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _strPort.Receive();
            if (p != null)
            {
                var str = p.Content.ToString()?.ToUpper();
                Drop(p);
                _strPort.Close();
                if (str != null)
                {
                    if(str == "XML") _structure = new ST.structure { which = ST.structure.WHICH.Xml };
                    else if (str == "JSON") _structure = new ST.structure { which = ST.structure.WHICH.Json };
                    else if (str == "NONE") _structure = new ST.structure { which = ST.structure.WHICH.None };
                }
            }

            p = _inPort.Receive();
            if (p != null)
            {
                var content = p.Content.ToString();
                Drop(p);
                if (content == null) return;
                var st = new ST() { Structure = _structure, Value = content };
                p = Create(st);
                _outPort.Send(p);
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _strPort = OpenInput("STR");
            _outPort = OpenOutput("OUT");
        }
    }
}
