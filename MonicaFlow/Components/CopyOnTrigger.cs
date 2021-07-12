using Capnp;
using FBPLib;
using System;
using System.Linq;
using System.Reflection;

namespace Components
{
    [InPort("COPY", description = "Values to be copied (ValueType) or Cloned (ICloneable)")]
    [InPort("TRIG", description = "Trigger port. When value arrives, a copy will be triggered")]
    [InPort("METH", description = "Clone method to call on VAL. Is optional. Default is to copy ValueTypes and clone only ICloneable objects.")]
    [OutPort("OUT", arrayPort = true)]
    [ComponentDescription("Copy COPY on every received packet on TRIG using method METH.")]
    class CopyOnTrigger : Component
    {
        IInputPort _copyPort;
        object _copy;
        IInputPort _trigPort;
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

            p = _copyPort.Receive();
            if (p != null)
            {
                _copy = p.Content;
                Drop(p);
            }

            // we need an object to copy before continuing
            if (_copy == null) return;

            p = _trigPort.Receive();
            if (p != null)
            {
                Drop(p);
                int no = _outPortArray.Length;
                if (!string.IsNullOrEmpty(_met))
                {
                    var t = _copy.GetType();
                    var m = t.GetMethod(_met);
                    for (int i = 0; i < no; i++)
                    {
                        var clone = m.Invoke(_copy, null);
                        if (clone != null) _outPortArray[i].Send(Create(clone));
                    }
                }
                else
                {
                    if (_copy is ValueType vt)
                        for (int i = 0; i < no; i++)
                            _outPortArray[i].Send(Create(vt));
                    else if (_copy is ICloneable c)
                        for (int i = 0; i < no; i++)
                            _outPortArray[i].Send(Create(c.Clone()));
                    else if (_copy is ICapnpSerializable obj)
                    {
                        var copyType = _copy.GetType();
                        var copyTypeWriter = copyType.GetNestedType("WRITER");
                        var msg = MessageBuilder.Create();
                        var msgType = msg.GetType();
                        var brMI = msgType.GetMethod("BuildRoot");
                        var brGMI = brMI.MakeGenericMethod(new Type[] { copyTypeWriter });
                        if (brGMI.Invoke(msg, null) is SerializerState root)
                        {
                            obj.Serialize(root);
                            var type = obj.GetType();
                            ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
                            for (int i = 0; i < no; i++)
                            {
                                if (ci.Invoke(null) is ICapnpSerializable copy)
                                {
                                    copy.Deserialize(root);
                                    _outPortArray[i].Send(Create(copy));
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void OpenPorts()
        {
            _copyPort = OpenInput("COPY");
            _trigPort = OpenInput("TRIG");
            _methPort = OpenInput("METH");
            _outPortArray = OpenOutputArray("OUT");
        }
    }
}
