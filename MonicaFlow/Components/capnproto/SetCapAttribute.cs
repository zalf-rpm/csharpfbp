using System;
using System.Linq;
using Mas.Rpc;
using Capnp.Rpc;
using FBPLib;

namespace Components
{
    [InPort("KEY", description = "The KEY under which to set the capability in the attributes of the IP. Defaults to 'KEY'.")]
    [InPort("CAP", description = "A capability.")]
    [InPort("IN", description = "IP in which to set the attribute.")]
    [OutPort("OUT")]
    [ComponentDescription("Receive capability and call it sending the result")]
    class SetCapAttribute : Component, IDisposable
    {
        IInputPort _keyPort;
        string _key = "KEY";
        IInputPort _capPort;
        Proxy _cap = null;
        IInputPort _inPort;
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

            if ((p = _capPort.Receive()) != null)
            {
                if (p.Content is Proxy cap)
                {
                    _cap?.Dispose(); // dispose potentially old capability
                    _cap = cap;
                }
                Drop(p);
            }

            if ((p = _inPort.Receive()) != null)
            {
                if (_cap != null)
                {
                    var itypes = (from t in _cap.GetType().GetInterfaces()
                                  from a in t.CustomAttributes
                                  where a.AttributeType == typeof(ProxyAttribute)
                                  select t);
                    if (itypes.Any())
                    {
                        var mi = typeof(Proxy).GetMethod("Share").MakeGenericMethod(new Type[] { itypes.First() });
                        var o = mi.Invoke(null, new object[] { _cap });
                        p.Attributes[_key] = o; //_cap.Cast<Proxy>(false);
                    }
                }
                //if (_cap != null) p.Attributes[_key] = Proxy.Share(_cap);
                _outPort.Send(p);
            }
        }

        public override void OpenPorts()
        {
            _capPort = OpenInput("CAP");
            _keyPort = OpenInput("KEY");
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }

        public void Dispose()
        {
            _cap?.Dispose();
        }
    }
}
