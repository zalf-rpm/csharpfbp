using FBPLib;
using System.Linq;

namespace Components
{
    [InPort("IN", description = "Values to be copied (ValueType) or Cloned (ICloneable)")]
    [OutPort("OUT", arrayPort = true)]
    [ComponentDescription("Duplicate value type or ICloneable ref type on IN amongst all connected ports to array port OUT.")]
    class Duplicate : Component
    {
        IInputPort _inPort;
        OutputPort[] _outPortArray;

        public override void Execute()
        {
            int no = _outPortArray.Length;

            Packet p;
            while((p = _inPort.Receive()) != null)
            {
                var o = p.Content;
                Drop(p);
                if (o is System.ValueType vt) for (int i = 0; i < no; i++) _outPortArray[i].Send(Create(vt));
                else if(o is System.ICloneable c) for (int i = 0; i < no; i++) _outPortArray[i].Send(Create(c.Clone()));
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPortArray = OpenOutputArray("OUT");
        }
    }
}
