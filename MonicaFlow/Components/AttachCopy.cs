using System;
using System.Collections.Generic;
using System.Linq;
using Capnp.Rpc;
using FBPLib;
using C = Mas.Rpc.Common;
using Model = Mas.Rpc.Model;
using Soil = Mas.Rpc.Soil;

namespace Components
{
    [InPort("KEY", description = "Key under which value to attach. Defaults to 'KEY'")]
    [InPort("VAL", description = "Value to attach")]
    [InPort("MET", description = "Clone method to call on VAL. Is optional. Default is to copy ValueTypes and clone only ICloneable objects.")]
    [InPort("IN", description = "Input packet to attach the soil profile to. Multiple profiles will try to deep copy the packet.")]
    [OutPort("OUT")]
    [ComponentDescription("Get the closest soil profiles to the given geo-location LATLON with the mandatory paramaters MAN and optionally OPT.")]
    class AttachCopy : Component
    {
        IInputPort _keyPort;
        string _key = "KEY";
        IInputPort _valPort;
        object _val = null;
        IInputPort _metPort;
        string _met = null;
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

            p = _valPort.Receive();
            if (p != null)
            {
                _val = p.Content;
                Drop(p);
                _valPort.Close();
            }

            p = _metPort.Receive();
            if (p != null)
            {
                _met = p.Content?.ToString();
                Drop(p);
                _metPort.Close();
            }

            while ((p = _inPort.Receive()) != null)
            {
                if (p.Type == Packet.Types.Normal)
                {
                    if (!string.IsNullOrEmpty(_met))
                    {
                        var t = _val.GetType();
                        var m = t.GetMethod(_met);
                        var clone = m.Invoke(_val, null);
                        if (clone != null) p.Attributes[_key] = clone;
                    }
                    else
                    {
                        if (_val is System.ValueType vt) p.Attributes[_key] = vt;
                        else if (_val is System.ICloneable c) p.Attributes[_key] = c.Clone();
                    }
                }
                _outPort.Send(p);
            }
        }

        public override void OpenPorts()
        {
            _keyPort = OpenInput("KEY");
            _valPort = OpenInput("VAL");
            _metPort = OpenInput("MET");
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }
    }
}
