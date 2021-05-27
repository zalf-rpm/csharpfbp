using FBPLib;

namespace Components
{
    [InPort("KEY", description = "Attribute KEY. Defaults to 'KEY'.")]
    [InPort("IN", description = "IP which contains an attribute with key KEY.")]
    [InPort("VAL", description = "The value to set at KEY.")]
    [OutPort("OUT")]
    [ComponentDescription("Set an attribute from IP at IN, forwarding the updated IP unchanged at OUT.")]
    class SetAttribute : Component
    {
        IInputPort _keyPort;
        string _key = "KEY";
        IInputPort _inPort;
        IInputPort _valPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _keyPort.Receive();
            if (p != null)
            {
                _key = p.Content?.ToString() ?? _key;
                Drop(p);
                _keyPort.Close();
            }
            
            object val = null;
            if ((p = _valPort.Receive()) != null)
            {
                val = p.Content;
                Drop(p);
            }

            if ((p = _inPort.Receive()) != null)
            {
                if(p.Type == Packet.Types.Normal) p.Attributes[_key] = val;
                _outPort.Send(p);
            }
        }

        public override void OpenPorts()
        {
            _keyPort = OpenInput("KEY");
            _inPort = OpenInput("IN");
            _valPort = OpenInput("VAL");
            _outPort = OpenOutput("OUT");
        }
    }
}
