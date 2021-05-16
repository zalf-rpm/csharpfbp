using System.IO;
using FBPLib;

namespace Components
{
    [InPort("IN", description = "Path to a text file")]
    [OutPort("OUT", type = typeof(string))]
    [ComponentDescription("Read text file at IN and send full content as string on OUT")]
    class ReadCompleteFile : Component
    {
        IInputPort _inPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _inPort.Receive();
            if (p != null)
            {
                var filepath = p.Content.ToString();
                Drop(p);
                if (filepath != null)
                {
                    try
                    {
                        var content = File.ReadAllText(filepath);
                        p = Create(content);
                        _outPort.Send(p);
                    }
                    catch (System.Exception) { }
                }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }
    }
}
