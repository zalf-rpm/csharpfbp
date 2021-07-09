using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Capnp.Rpc;
using FBPLib;
using Mas.Rpc.Geo;
using Soil = Mas.Rpc.Soil;

namespace Components
{
    [InPort("CAP", description = "Soil service capability", type = typeof(Soil.IService))]
    [InPort("MAN", description = "Mandatory parameters (a list)")]
    [InPort("OPT", description = "Optional parameters (a list)")]
    [InPort("IN", description = "Input packet to attach the soil profile to. Multiple profiles will try to deep copy the packet.")]
    [OutPort("OUT")]
    [ComponentDescription("Get the closest soil profiles to the given geo-location LATLON with the mandatory paramaters MAN and optionally OPT.")]
    class AttachSoilProfiles : Component
    {
        IInputPort _capPort;
        Soil.IService _cap;
        IInputPort _manPort;
        List<Soil.PropertyName> _manProps = new();
        IInputPort _optPort;
        List<Soil.PropertyName> _optProps = new();
        IInputPort _inPort;
        OutputPort _outPort;

        private List<Soil.PropertyName> PropListFromStrings(IEnumerable<string> props)
        {
            List<Soil.PropertyName> res = new();
            foreach (Soil.PropertyName prop in Enum.GetValues(typeof(Soil.PropertyName)))
                if (props.Contains(prop.ToString())) res.Add(prop);
            return res;
        }

        public override void Execute()
        {
            Packet p = _capPort.Receive();
            if (p != null)
            {
                _cap = p.Content as Soil.IService;
                Drop(p);
                _capPort.Close();
            }

            p = _manPort.Receive();
            if (p != null)
            {
                var manStr = p.Content.ToString();
                Drop(p);
                _manProps = PropListFromStrings(manStr.Split(",").Select(s => s.Trim()));
                _manPort.Close();
            }

            p = _optPort.Receive();
            if (p != null)
            {
                var optStr = p.Content.ToString();
                Drop(p);
                _optProps = PropListFromStrings(optStr.Split(",").Select(s => s.Trim()));
                _optPort.Close();
            }

            while ((p = _inPort.Receive()) != null)
            {
                if (_cap != null && p.Content is LatLonCoord llc) 
                {
                    try
                    {
                        var profs = _cap.ProfilesAt(
                            llc,
                            new Soil.Query { Mandatory = _manProps, Optional = _optProps, OnlyRawData = false }
                        ).Result;

                        if (profs.Count > 1)
                        {
                            foreach (var prof in profs.SkipLast(1))
                            {
                                Packet np = Create(llc);
                                foreach (var (key, val) in p.Attributes)
                                {
                                    if (val is System.ValueType vt2) np.Attributes[key] = vt2;
                                    else if (val is System.ICloneable c) np.Attributes[key] = c.Clone();
                                }
                                np.Attributes["soil-profile"] = prof;
                                _outPort.Send(np);
                            }
                        }
                        p.Attributes["soil-profile"] = profs.Last();
                        _outPort.Send(p);
                    }
                    catch (RpcException e) { Console.WriteLine(e.Message); }
                }
                else // send packet unchanged to OUT
                {
                    _outPort.Send(p);
                }
            }

            //_cap?.Dispose();
        }

        public override void OpenPorts()
        {
            _capPort = OpenInput("CAP");
            _manPort = OpenInput("MAN");
            _optPort = OpenInput("OPT");
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }

        public override void Dispose()
        {
            _cap?.Dispose();
        }
    }
}
