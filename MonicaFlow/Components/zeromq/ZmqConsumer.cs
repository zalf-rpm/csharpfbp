using FBPLib;
using System.Linq;
using NetMQ.Sockets;
using NetMQ;
using System;

namespace Components
{
    [InPort("CON", description = "IIP for the connection string for this pull socket")]
    [OutPort("OUT")]
    [ComponentDescription("Simply forward all received messages from the connection")]
    class ZmqConsumer : Component
    {
        IInputPort _conPort;
        string _address;
        PullSocket _pullSocket = new();
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _conPort.Receive();
            if (p != null)
            {
                _address = p.Content.ToString();
                Drop(p);
                _pullSocket.Connect(_address);
                _conPort.Close();
            }
            
            while(true)
            {
                string msg;
                if (_pullSocket.TryReceiveFrameString(new TimeSpan(0, 0, 1), out msg))
                {
                    if (_outPort.IsConnected()) _outPort.Send(Create(msg));
                    else break;
                }
                else if (!_outPort.IsConnected()) break;
            }
        }

        public override void OpenPorts()
        {
            _conPort = OpenInput("CON");
            _outPort = OpenOutput("OUT");
        }
        public void Dispose()
        {
            _pullSocket?.Disconnect(_address);
        }
    }
}
