using System;
using FBPLib;

namespace Components
{
    [InPort("IN", description = "Object with properties")]
    [InPort("PROP", description = "Property to access in object IN")]
    [OutPort("OUT")]
    [ComponentDescription("Get the value from property with name PROP from object received on IN")]
    class GetProperty : Component
    {
        IInputPort _inPort;
        object _obj = null;
        IInputPort _propPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _inPort.Receive();
            if (p != null)
            {
                _obj = p.Content;
                Drop(p);
            }

            // we need a struct in order to continue
            if (_obj == null) return;

            p = _propPort.Receive();
            if (p != null)
            {
                var propName = p.Content?.ToString() ?? "__";
                var t = _obj.GetType();
                var propInfo = t.GetProperty(propName);
                Drop(p);
                if (propInfo != null)
                {
                    try
                    {
                        var val = propInfo.GetValue(_obj);
                        if (val != null) _outPort.Send(Create(val));
                    }
                    catch (System.Exception e) 
                    {
                        Console.WriteLine("Exception in GetProperty: " + e.Message);
                    }
                }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _propPort = OpenInput("PROP");
            _outPort = OpenOutput("OUT");
        }
    }
}
