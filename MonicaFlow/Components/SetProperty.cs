using System;
using System.Reflection;
using FBPLib;

namespace Components
{
    [InPort("IN", description = "Object with properties")]
    [InPort("PROP", description = "Property to access in object IN")]
    [InPort("VAL", description = "Value to set on property PROP in object IN")]
    [OutPort("OUT")]
    [ComponentDescription("Set the value VAL on property with name PROP in object received on IN")]
    class SetProperty : Component
    {
        IInputPort _inPort;
        object _obj = null;
        IInputPort _propPort;
        PropertyInfo _propInfo;
        IInputPort _valPort;
        object _val = null;
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
                _propInfo = t.GetProperty(propName);
                Drop(p);
            }

            // we need a property to continue
            if (_propInfo == null) return;
             
            p = _valPort.Receive();
            if (p != null)
            {
                var val = p.Content;
                Drop(p);
                try
                {
                    _propInfo.SetValue(_obj, val);
                    _outPort.Send(Create(_obj));
                    _obj = null;
                }
                catch (System.Exception e) 
                {
                    Console.WriteLine("Exception in SetProperty: " + e.Message);
                }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _propPort = OpenInput("PROP");
            _valPort = OpenInput("VAL");
            _outPort = OpenOutput("OUT");
        }
    }
}
