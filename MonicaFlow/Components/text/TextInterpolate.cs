using FBPLib;

namespace Components
{
    [InPort("IN", description = "Value to be put into format string STR.")]
    [InPort("TEMP", description = "Template TEMP to be formated with IN. Placeholder is ${IN}", type = typeof(string))]
    [OutPort("OUT", type = typeof(string))]
    [ComponentDescription("Replace ${IN} in template TEMP with value coming in at IN.")]
    class TextInterpolate : Component
    {
        IInputPort _inPort;
        IInputPort _tempPort;
        string _temp;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _tempPort.Receive();
            if (p != null)
            {
                _temp = p.Content?.ToString();
                Drop(p);
            }

            // continue only if we got a template
            if (_temp == null) return;

            while ((p = _inPort.Receive()) != null)
            {
                var val = p.Content?.ToString();
                Drop(p);
                if (val != null)
                {
                    var repl = _temp.Replace("${IN}", val);
                    _outPort.Send(Create(repl));
                }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _tempPort = OpenInput("TEMP");
            _outPort = OpenOutput("OUT");
        }
    }
}
