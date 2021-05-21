using FBPLib;
using System.Linq;

namespace Components
{
    [InPort("IN", description = "input string to be split", type = typeof(string))]
    [InPort("AT", description = "token to use for splitting, by default ','", type = typeof(string))]
    [OutPort("OUT", type = typeof(string))]
    [ComponentDescription("Split string arriving at IN at tokens AT and send split parts to OUT")]
    class SplitString : Component
    {
        IInputPort _inPort;
        IInputPort _atPort;
        string _at = ",";
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _atPort.Receive();
            if (p != null)
            {
                _at = p.Content.ToString();
                Drop(p);
                _atPort.Close();
            }
             
            while((p = _inPort.Receive()) != null)
            {
                var str = p.Content.ToString();
                Drop(p);
                var strs = str.Split(_at).Select(s => s.Trim());
                if (strs.Count() > 1) _outPort.Send(Create(Packet.Types.Open, ""));
                foreach(var s in strs) _outPort.Send(Create(s));
                if (strs.Count() > 1) _outPort.Send(Create(Packet.Types.Close, ""));
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _atPort = OpenInput("AT");
            _outPort = OpenOutput("OUT");
        }
    }
}
