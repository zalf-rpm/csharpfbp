using FBPLib;
using System;
using System.Linq;
using System.Reflection;

namespace Components
{
    [InPort("IN", description = "Values to be copied (ValueType) or Cloned (ICloneable)")]
    [InPort("METH", description = "Clone method to call on VAL. Is optional. Default is to copy ValueTypes and clone only ICloneable objects.")]
    [OutPort("OUT", arrayPort = true)]
    [ComponentDescription("Duplicate value type or ICloneable ref type on IN amongst all connected ports to array port OUT.")]
    class Duplicate : Component
    {
        IInputPort _inPort;
        IInputPort _methPort;
        string _met = null;
        OutputPort[] _outPortArray;

        public override void Execute()
        {
            Packet p = _methPort.Receive();
            if (p != null)
            {
                _met = p.Content?.ToString();
                Drop(p);
            }
            
            p = _inPort.Receive();
            if (p != null)
            {
                var obj = p.Content;
                Drop(p);
                if (obj != null)
                {
                    int no = _outPortArray.Length;
                    if (!string.IsNullOrEmpty(_met))
                    {
                        var t = obj.GetType();
                        var m = t.GetMethod(_met);
                        var clone = m.Invoke(obj, null);
                        if (clone != null) 
                            for (int i = 0; i < no; i++) 
                                _outPortArray[i].Send(Create(clone));
                    }
                    else
                    {
                        if (obj is System.ValueType vt) 
                            for (int i = 0; i < no; i++) 
                                _outPortArray[i].Send(Create(vt));
                        else if (obj is System.ICloneable c) 
                            for (int i = 0; i < no; i++) 
                                _outPortArray[i].Send(Create(c.Clone()));
                    }
                }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _methPort = OpenInput("METH");
            _outPortArray = OpenOutputArray("OUT");
        }
    }
}
