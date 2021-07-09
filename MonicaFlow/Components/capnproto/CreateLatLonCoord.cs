using System;
using System.Linq;
using FBPLib;
using System.Globalization;
using Mas.Rpc.Geo;

namespace Components
{
    [InPort("IN", description = "Lat,Lng string", type = typeof(string))]
    [InPort("LAT", description = "Latitude string or double")]
    [InPort("LON", description = "Longitude string or double")]
    [OutPort("OUT")]
    [ComponentDescription("Create a geo_coord.capnp:LatLonCoord from string at IN or data at LAT/LON")]
    class CreateLatLonCoord : Component
    {
        IInputPort _inPort;
        IInputPort _latPort;
        IInputPort _lonPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _inPort.Receive();
            if (p != null)
            {
                var str = p.Content.ToString();
                Drop(p);
                try
                {
                    var ll = str.Split(',').Select(v => double.Parse(v, CultureInfo.CreateSpecificCulture("en-US"))).ToArray();
                    _outPort.Send(Create(new LatLonCoord { Lat = ll[0], Lon = ll[1] }));
                }
                catch (System.Exception e) 
                { 
                    Console.WriteLine("Exception in CreateLatLonCoord receiving []: [" + str + "] Exception: " + e.Message); 
                }
            }

            Packet latp = _latPort.Receive();
            if (latp == null)
            {
                // close also lonPort, because lat/lon have to go together
                _lonPort.Close();
            }
            else
            {

                Packet lonp = _lonPort.Receive();
                if (lonp == null)
                {
                    if (latp != null)
                    {
                        Drop(latp); // lat and lon packets have to go together
                        _latPort.Close();
                    }
                }
                else
                {
                    var lat = (double)(latp.Content as Newtonsoft.Json.Linq.JValue);
                    Drop(latp);
                    var lon = (double)(lonp.Content as Newtonsoft.Json.Linq.JValue);
                    Drop(lonp);
                    _outPort.Send(Create(new LatLonCoord { Lat = lat, Lon = lon }));
                }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _latPort = OpenInput("LAT");
            _lonPort = OpenInput("LON");
            _outPort = OpenOutput("OUT");
        }
    }
}
