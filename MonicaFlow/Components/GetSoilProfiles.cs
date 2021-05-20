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
    [InPort("CAP", description = "Soil service capability", type = typeof(Soil.IService))]
    [InPort("MAN", description = "Mandatory parameters (a list)")]
    [InPort("OPT", description = "Optional parameters (a list)")]
    [InPort("LATLON", description = "Geo-location of profile")]
    [OutPort("OUT")]
    [ComponentDescription("Get the closest soil profiles to the given geo-location LATLON with the mandatory paramaters MAN and optionally OPT.")]
    class GetSoilProfiles : Component, IDisposable
    {
        IInputPort _capPort;
        Soil.IService _cap;
        IInputPort _manPort;
        List<Soil.PropertyName> _manProps = new();
        IInputPort _optPort;
        List<Soil.PropertyName> _optProps = new();
        IInputPort _latlonPort;
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

            while ((p = _latlonPort.Receive()) != null && _cap != null)
            {
                //if (p.Type != Packet.Types.Normal)
                if (p.Content.ToString() == "Open" || p.Content.ToString() == "Close")
                {
                    Drop(p);
                    continue;
                }
                var latlon = p.Content.ToString()?.Split(",");
                Drop(p);
                if (latlon.Length > 1)
                {
                    var lat = double.Parse(latlon[0]);
                    var lon = double.Parse(latlon[1]);

                    try
                    {
                        var profs = _cap.ProfilesAt(
                            new Mas.Rpc.Geo.LatLonCoord { Lat = lat, Lon = lon },
                            new Soil.Query { Mandatory = _manProps, Optional = _optProps, OnlyRawData = false }
                        ).Result;
                        
                        if (profs.Count() > 1) _outPort.Send(Create(Packet.Types.Open, ""));
                        foreach (var prof in profs) _outPort.Send(Create(prof));
                        if (profs.Count() > 1) _outPort.Send(Create(Packet.Types.Close, ""));
                    }
                    catch (RpcException e) { Console.WriteLine(e.Message); }
                }
            }
        }

        public override void OpenPorts()
        {
            _capPort = OpenInput("CAP");
            _manPort = OpenInput("MAN");
            _optPort = OpenInput("OPT");
            _latlonPort = OpenInput("LATLON");
            _outPort = OpenOutput("OUT");
        }

        public void Dispose()
        {
            _cap?.Dispose();
        }
    }
}
