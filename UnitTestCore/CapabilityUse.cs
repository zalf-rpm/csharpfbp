﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mas.Rpc;
using Capnp.Rpc;
using FBPLib;

namespace UnitTests
{
    [InPort("IN", type=typeof(Mas.Rpc.Climate.ITimeSeries))]
    [OutPort("OUT")]
    [ComponentDescription("Receive capability and call it sending the result")]
    class CapabilityUse : Component, IDisposable
    {
        IInputPort _inPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _inPort.Receive();
            Mas.Rpc.Climate.ITimeSeries timeSeries = null;
            if (p != null)
            {
                timeSeries = p.Content as Mas.Rpc.Climate.ITimeSeries;
                Drop(p);
                _inPort.Close();
            }

            if (timeSeries != null)
            {
                try
                {
                    var header = timeSeries.Header().Result;
                    var hl = header.Select(h => h.ToString()).ToList();
                    //Console.WriteLine(hl.Aggregate((a, v) => a + " | " + v));
                    p = Create(hl);
                    _outPort.Send(p);
                    timeSeries.Dispose();
                }
                catch (RpcException e) { Console.WriteLine(e.Message); }
            }
        }

        public override void OpenPorts()
        {
            _inPort = OpenInput("IN");
            _outPort = OpenOutput("OUT");
        }

        public void Dispose()
        {
        }
    }
}
