﻿using System;
using System.Linq;
using Climate = Mas.Rpc.Climate;
using Soil = Mas.Rpc.Soil;
using Mgmt = Mas.Rpc.Management;
using Model = Mas.Rpc.Model;
using ST = Mas.Rpc.Common.StructuredText;
using Capnp.Rpc;
using FBPLib;
using System.Collections.Generic;

namespace Components
{
    public class Class2
    {
        public static T Cast<T>(object o)
        {
            return (T)o;
        }
    }

    [InPort("REST", description = "RestInput")]
    [InPort("TS", description = "Mas.Rpc.Climate.ITimeSeries capability")]
    [InPort("SP", description = "Mas.Rpc.Soil.Profile structure")]
    [InPort("MES", description = "Enumerable of Mas.Rpc.Management.Event ")]
    [OutPort("OUT")]
    [ComponentDescription("Receive capability and call it sending the result")]
    class CreateModelEnv : Component
    {
        IInputPort _restPort;
        IInputPort _timeSeriesPort;
        IInputPort _soilProfilePort;
        IInputPort _mgmtEventsPort;
        OutputPort _outPort;

        public override void Execute()
        {
            Packet p = _restPort.Receive();
            //Capnp.AnyPointer rest;
            object rest = null;
            ST restST = null;
            if (p != null)
            {
                rest = p.Content;
                Drop(p);
                restST = rest as ST;
            }

            p = _timeSeriesPort.Receive();
            Climate.ITimeSeries timeSeries = null;
            if (p != null)
            {
                timeSeries = p.Content as Climate.ITimeSeries;
                Drop(p);
            }

            p = _soilProfilePort.Receive();
            Soil.Profile soilProfile = null;
            if (p != null)
            {
                soilProfile = p.Content as Soil.Profile;
                Drop(p);
            }

            p = _mgmtEventsPort.Receive();
            IEnumerable<Mgmt.Event> mgmtEvents = null;
            if (p != null)
            {
                mgmtEvents = p.Content as IEnumerable<Mgmt.Event>;
                Drop(p);
            }

            // we need the rest in order to create the Env, else we deactivate the component
            if (rest == null) return;

            var restType = rest.GetType();
            var envType = typeof(Model.Env<>).MakeGenericType(restType);
            dynamic env = Activator.CreateInstance(envType);

            //var selfType = typeof(Class2);
            //var methodInfo = selfType.GetMethod("Cast", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            //var genericArguments = new[] { restType };
            //var genericMethodInfo = methodInfo?.MakeGenericMethod(genericArguments);
            //var restC = genericMethodInfo?.Invoke(null, new[] { rest });
            //Console.WriteLine(restC.GetType());

            if (restST != null) env.Rest = restST; // restC;
            if (timeSeries != null) env.TimeSeries = timeSeries;
            if (soilProfile != null) env.SoilProfile = soilProfile;
            if (mgmtEvents != null) env.cropRotation = mgmtEvents;

            p = Create(env);
            _outPort.Send(p);
        }

        public override void OpenPorts()
        {
            _restPort = OpenInput("REST");
            _timeSeriesPort = OpenInput("TS");
            _soilProfilePort = OpenInput("SP");
            _mgmtEventsPort = OpenInput("MES");
            _outPort = OpenOutput("OUT");
        }
    }
}
