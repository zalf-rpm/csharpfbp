﻿using System;
using FBPLib;
using Components;
using System.Threading.Tasks;

namespace MonicaFlow
{ 
    public class MonicaFlow : CapabilityNetwork
    {
        string description = "Click anywhere on selection area";
        public override void Define()
        {
            //*
            Component("create_csvs", typeof(MonicaCreateCSV));
            Component("WriteConsole", typeof(WriteToConsole));
            Component("Read__sim.json", typeof(ReadCompleteFile));
            Component("Replace_env_var", typeof(ReplaceEnvVars));
            Component("Read__crop.json", typeof(ReadCompleteFile));
            Component("Replace__env_var", typeof(ReplaceEnvVars));
            Component("Read__site.json", typeof(ReadCompleteFile));
            Component("Replace_env_var_2_", typeof(ReplaceEnvVars));
            Component("Monica__Patch", typeof(MonicaPatchSubnet));
            Component("Split", typeof(SplitString));
            Connect(Component("Read__site.json"), Port("OUT"), Component("Monica__Patch"), Port("SITE"));
            Initialize("capnp://localhost:10000", Component("Monica__Patch"), Port("SOILSERVICE_SR"));
            Initialize("|", Component("Split"), Port("AT"));
            Initialize("50,10 | 51,11 | 52,12", Component("Split"), Port("IN"));
            Connect(Component("Split"), Port("OUT"), Component("Monica__Patch"), Port("LATLON"));
            Initialize("{\"split\": true, \"csvSep\": \";\"}", Component("create_csvs"), Port("OPTS"));
            Connect(Component("Monica__Patch"), Port("OUT"), Component("create_csvs"), Port("IN"));
            Connect(Component("create_csvs"), Port("OUT"), Component("WriteConsole"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\sim-min.json", Component("Replace_env_var"), Port("IN"));
            Connect(Component("Replace_env_var"), Port("OUT"), Component("Read__sim.json"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\crop-min.json", Component("Replace__env_var"), Port("IN"));
            Connect(Component("Replace__env_var"), Port("OUT"), Component("Read__crop.json"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\site-min.json", Component("Replace_env_var_2_"), Port("IN"));
            Connect(Component("Replace_env_var_2_"), Port("OUT"), Component("Read__site.json"), Port("IN"));
            Initialize("capnp://localhost:11002", Component("Monica__Patch"), Port("TIMESERIES_SR"));
            Initialize("capnp://localhost:6666", Component("Monica__Patch"), Port("MONICA_SR"));
            Connect(Component("Read__sim.json"), Port("OUT"), Component("Monica__Patch"), Port("SIM"));
            Connect(Component("Read__crop.json"), Port("OUT"), Component("Monica__Patch"), Port("CROP"));
            //*/


            /*
            Component("WriteConsole", typeof(WriteToConsole));
            Component("Read_sim.json", typeof(ReadCompleteFile));
            Component("Read_crop.json", typeof(ReadCompleteFile));
            Component("Read_site.json", typeof(ReadCompleteFile));
            Component("Monica_Patch", typeof(MonicaPatchSubnet));
            Component("Split", typeof(SplitString));
            Component("Select_oids", typeof(SelectJsonElement));
            Connect(Component("Read_site.json"), Port("OUT"), Component("Monica_Patch"), Port("SITE"));
            Initialize("capnp://localhost:10000", Component("Monica_Patch"), Port("SOILSERVICE_SR"));
            Initialize("|", Component("Split"), Port("AT"));
            Initialize("50,10 | 51,11 | 52,12", Component("Split"), Port("IN"));
            Connect(Component("Split"), Port("OUT"), Component("Monica_Patch"), Port("LATLON"));
            Connect(Component("Monica_Patch"), Port("SECTIONS"), Component("Select_oids"), Port("IN"));
            Initialize("{\"key\": \"outputIds\"}", Component("Select_oids"), Port("CONF"));
            Connect(Component("Select_oids"), Port("OUT"), Component("WriteConsole"), Port("IN"));
            Initialize("C:\\Users\\micha\\MONICA\\Examples\\Hohenfinow2\\sim-min.json", Component("Read_sim.json"), Port("IN"));
            Initialize("C:\\Users\\micha\\MONICA\\Examples\\Hohenfinow2\\crop-min.json", Component("Read_crop.json"), Port("IN"));
            Initialize("C:\\Users\\micha\\MONICA\\Examples\\Hohenfinow2\\site-min.json", Component("Read_site.json"), Port("IN"));
            Initialize("capnp://localhost:11002", Component("Monica_Patch"), Port("TIMESERIES_SR"));
            Initialize("capnp://localhost:6666", Component("Monica_Patch"), Port("MONICA_SR"));
            Connect(Component("Read_sim.json"), Port("OUT"), Component("Monica_Patch"), Port("SIM"));
            Connect(Component("Read_crop.json"), Port("OUT"), Component("Monica_Patch"), Port("CROP"));
            //*/

            /*
            Component("TimeSeries", typeof(ConnectToSturdyRef));
            Component("Run_Monica", typeof(RunEnvModel));
            Component("WriteConsole", typeof(WriteToConsole));
            Component("MONICA", typeof(ConnectToSturdyRef));
            Component("Create_Env", typeof(CreateModelEnv));
            Component("Create_JSON__Rest_Env", typeof(CreateMonicaEnv));
            Component("Read__sim.json", typeof(ReadCompleteFile));
            Component("Read__crop.json", typeof(ReadCompleteFile));
            Component("Read__site.json", typeof(ReadCompleteFile));
            Component("Make__Structured__Text", typeof(CreateStructuredText));
            Component("Create_sim_object", typeof(CreateJsonObject));
            Component("Create_crop_object", typeof(CreateJsonObject));
            Component("Create_site_object", typeof(CreateJsonObject));
            Connect(Component("Create_crop_object"), Port("OUT"), Component("Create_JSON__Rest_Env"), Port("CROP"));
            Connect(Component("Create_site_object"), Port("OUT"), Component("Create_JSON__Rest_Env"), Port("SITE"));
            Connect(Component("Run_Monica"), Port("OUT"), Component("WriteConsole"), Port("IN"));
            Initialize("capnp://localhost:11002", Component("TimeSeries"), Port("SR"));
            Initialize("capnp://localhost:6666", Component("MONICA"), Port("SR"));
            Connect(Component("MONICA"), Port("OUT"), Component("Run_Monica"), Port("CAP"));
            Connect(Component("Create_Env"), Port("ENV"), Component("Run_Monica"), Port("ENV"));
            Initialize(Environment.GetEnvironmentVariable("MONICA_HOME") + "\\Examples\\Hohenfinow2\\sim-min.json", Component("Read__sim.json"), Port("IN"));
            Initialize(Environment.GetEnvironmentVariable("MONICA_HOME") + "\\Examples\\Hohenfinow2\\crop-min.json", Component("Read__crop.json"), Port("IN"));
            Initialize(Environment.GetEnvironmentVariable("MONICA_HOME") + "\\Examples\\Hohenfinow2\\site-min.json", Component("Read__site.json"), Port("IN"));
            Connect(Component("Create_JSON__Rest_Env"), Port("ENV"), Component("Make__Structured__Text"), Port("IN"));
            Initialize("JSON", Component("Make__Structured__Text"), Port("STR"));
            Connect(Component("Make__Structured__Text"), Port("OUT"), Component("Create_Env"), Port("REST"));
            Initialize("EnvInstance<StructuredText,StructuredText>", Component("MONICA"), Port("CT"));
            Initialize("TimeSeries", Component("TimeSeries"), Port("CT"));
            Connect(Component("TimeSeries"), Port("OUT"), Component("Create_Env"), Port("TS"));
            Connect(Component("Read__sim.json"), Port("OUT"), Component("Create_sim_object"), Port("IN"));
            Connect(Component("Read__crop.json"), Port("OUT"), Component("Create_crop_object"), Port("IN"));
            Connect(Component("Read__site.json"), Port("OUT"), Component("Create_site_object"), Port("IN"));
            Connect(Component("Create_sim_object"), Port("OUT"), Component("Create_JSON__Rest_Env"), Port("SIM"));
            //*/
        }
    }
}
