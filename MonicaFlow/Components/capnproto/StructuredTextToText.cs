using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using FBPLib;
using ST = Mas.Rpc.Common.StructuredText;

namespace Components
{
    [InPort("IN", description = "StructuredText IP")]
    [InPort("TEXT", description = "output only text (true/yes | false/no = default")]
    [OutPort("OUT")]
    [ComponentDescription("Extract text out of StructuredText in IP")]
    class StructuredTextToText : Component
    {
        IInputPort _inPort;
        IInputPort _textPort;
        bool _text = false;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _textPort.Receive();
            if (p != null)
            {
                var c = p.Content?.ToString().ToUpper();
                _text = c == "TRUE" || c == "YES";
                Drop(p);
                _textPort.Close();
            }

            if ((p = _inPort.Receive()) != null)
            {
                if(p.Content is ST st)
                {
                    if (_text) _outPort.Send(Create(st.Value));
                    else
                    {
                        switch (st.Structure.which)
                        {
                            case ST.structure.WHICH.Json:
                                var jt = JToken.Parse(st.Value);
                                _outPort.Send(Create(jt));
                                break;
                            case ST.structure.WHICH.Xml:
                                var xml = XDocument.Parse(st.Value);
                                _outPort.Send(Create(xml));
                                break;
                            case ST.structure.WHICH.None:
                                _outPort.Send(Create(st.Value));
                                break;
                            default:
                                _outPort.Send(Create(st.Value));
                                break;
                        }
                    }
                }
                Drop(p);
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _textPort = OpenInput("TEXT");
            _outPort = OpenOutput("OUT");
        }
    }
}
