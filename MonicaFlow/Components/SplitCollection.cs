using FBPLib;
using System.Collections;

namespace Components
{
    [InPort("IN", description = "IP to split", type = typeof(IEnumerable))]
    [OutPort("OUT", arrayPort = true)]
    [ComponentDescription("Split the enumerable json ip at IN round-robbing into out ports")]
    class SplitCollection : Component
    {
        IInputPort _inPort;
        OutputPort[] _outPort;

        public override void Execute()
        {
            int portCount = _outPort.Length;
            Packet p;
            while ((p = _inPort.Receive()) != null)
            {
                if (p.Content is IEnumerable elems)
                {
                    int i = 0;
                    foreach (var elem in elems)
                    {
                        if (i >= portCount) i = 0;
                        _outPort[i].Send(Create(elem));
                        i++;
                    }
                }
                Drop(p);
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPort = OpenOutputArray("OUT");
        }
    }
}
