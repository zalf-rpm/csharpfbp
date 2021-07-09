using Capnp;
using FBPLib;
using Mas.Rpc.Geo;
using System;
using System.Linq;
using System.Reflection;

namespace Components
{
    [InPort("IN", description = "Capnp objects to be copied")]
    [OutPort("OUT", arrayPort = true)]
    [ComponentDescription("Duplicate capnproto objects via serialization->deserialization.")]
    class CapnpDuplicate : Component
    {
        IInputPort _inPort;
        OutputPort[] _outPortArray;

        public override void Execute()
        {
            Packet p = _inPort.Receive();
            if (p != null)
            {
                if (p.Content is ICapnpSerializable obj)
                {
                    int no = _outPortArray.Length;
                    var msg = MessageBuilder.Create();
                    var root = msg.BuildRoot<LatLonCoord.WRITER>();
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
                Drop(p);
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPortArray = OpenOutputArray("OUT");
        }
    }
}
