using FBPLib;

namespace Components
{
    [InPort("KEY", description = "Attribute KEY. Defaults to 'KEY'")]
    [InPort("IN", description = "IP which contains an attribute with key KEY")]
    [OutPort("VAL")]
    [OutPort("OUT", optional = true)]
    [ComponentDescription("Get an attribute from IP at IN, forwarding the IP unchanged at OUT and the value under KEY on the VAL port.")]
    class GetAttribute : Component
    {
        IInputPort _keyPort;
        string _key = "KEY";
        IInputPort _inPort;
        OutputPort _valPort;
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

            p = _inPort.Receive();
            if (p != null)
            {
                if(p.Type == Packet.Types.Normal && p.Attributes.ContainsKey(_key)) _valPort.Send(Create(p.Attributes[_key]));
                if (_outPort.IsConnected()) _outPort.Send(p);
                else Drop(p);
            }
        }

        public override void OpenPorts()
        {
            _keyPort = OpenInput("KEY");
            _inPort = OpenInput("IN");
            _valPort = OpenOutput("VAL");
            _outPort = OpenOutput("OUT");
        }
    }
}
