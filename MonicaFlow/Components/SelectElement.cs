using FBPLib;
using System.Collections.Generic;
using System.Linq;

namespace Components
{
    [InPort("IN", description = "An enumerable", type = typeof(IEnumerable<object>))]
    [InPort("NO", description = "Element to select. default = 0 = first element", type = typeof(int))]
    [OutPort("OUT")]
    [ComponentDescription("Select a particular element from enumerable at IN")]
    class SelectElement : Component
    {
        IInputPort _inPort;
        IInputPort _noPort;
        int _at = 0;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _noPort.Receive();
            if (p != null)
            {
                _ = int.TryParse(p.Content?.ToString(), out _at);
                Drop(p);
                _noPort.Close();
            }

            while ((p = _inPort.Receive()) != null)
            {
                if (p.Content is IEnumerable<object> e)
                {
                    if (e.Count() >= _at) _outPort.Send(Create(e.ElementAt(_at)));
                }
                Drop(p);
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _noPort = OpenInput("NO");
            _outPort = OpenOutput("OUT");
        }
    }
}
