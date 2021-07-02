using System;
using FBPLib;
using Components;
using System.Threading.Tasks;

namespace MonicaFlow
{ 
    public class MonicaFlow5 : CapabilityNetwork
    {
        string description = "Click anywhere on selection area";
        public override void Define()
        {
            Component("Create_sim_object", typeof(CreateJsonObject));
            Component("WriteConsole", typeof(WriteToConsole));
            Component("Create_crop_object", typeof(CreateJsonObject));
            Component("Create_site_object", typeof(CreateJsonObject));
            Component("Create__MONICA_env", typeof(CreateMonicaEnv));
            Component("DUP", typeof(Duplicate));
            Component("dup_timeseries_sturdy_ref", typeof(Duplicate));
            Component("dup_soilservice_sturdy_ref", typeof(Duplicate));
            Component("dup_MONICA_sturdy_ref", typeof(Duplicate));
            Component("Read__sim.json", typeof(ReadCompleteFile));
            Component("Read__crop.json", typeof(ReadCompleteFile));
            Component("Read__site.json", typeof(ReadCompleteFile));
            Component("Monica_0", typeof(MonicaPatchSubnetV5));
            Component("Split", typeof(SplitString));
            Component("create_csvs", typeof(CreateMonicaCSV));
            Component("Replace__env_var", typeof(ReplaceEnvVars));
            Component("Replace___env_var", typeof(ReplaceEnvVars));
            Component("Replace__env_var_2_", typeof(ReplaceEnvVars));
            Component("Load_Balance", typeof(LoadBalance));
            Component("MONICA_1", typeof(MonicaPatchSubnetV5));
            Connect(Component("DUP"), Port("OUT[1]"), Component("MONICA_1"), Port("ENV"));
            Connect(Component("dup_timeseries_sturdy_ref"), Port("OUT[0]"), Component("Monica_0"), Port("TIMESERIES_SR"));
            Initialize("capnp://localhost:11002", Component("dup_timeseries_sturdy_ref"), Port("IN"));
            Connect(Component("dup_timeseries_sturdy_ref"), Port("OUT[1]"), Component("MONICA_1"), Port("TIMESERIES_SR"));
            Initialize("capnp://localhost:10000", Component("dup_soilservice_sturdy_ref"), Port("IN"));
            Connect(Component("dup_soilservice_sturdy_ref"), Port("OUT[0]"), Component("Monica_0"), Port("SOILSERVICE_SR"));
            Connect(Component("dup_soilservice_sturdy_ref"), Port("OUT[1]"), Component("MONICA_1"), Port("SOILSERVICE_SR"));
            Initialize("capnp://localhost:6666", Component("dup_MONICA_sturdy_ref"), Port("IN"));
            Connect(Component("dup_MONICA_sturdy_ref"), Port("OUT[0]"), Component("Monica_0"), Port("MONICA_SR"));
            Connect(Component("dup_MONICA_sturdy_ref"), Port("OUT[1]"), Component("MONICA_1"), Port("MONICA_SR"));
            Connect(Component("MONICA_1"), Port("OUT"), Component("create_csvs"), Port("IN"));
            Initialize("|", Component("Split"), Port("AT"));
            Initialize("50,10 | 51,11 | 52,12", Component("Split"), Port("IN"));
            Initialize("{\"split\": true, \"csvSep\": \";\"}", Component("create_csvs"), Port("OPTS"));
            Connect(Component("Monica_0"), Port("OUT"), Component("create_csvs"), Port("IN"));
            Connect(Component("create_csvs"), Port("OUT"), Component("WriteConsole"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\sim-min.json", Component("Replace__env_var"), Port("IN"));
            Connect(Component("Replace__env_var"), Port("OUT"), Component("Read__sim.json"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\crop-min.json", Component("Replace___env_var"), Port("IN"));
            Connect(Component("Replace___env_var"), Port("OUT"), Component("Read__crop.json"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\site-min.json", Component("Replace__env_var_2_"), Port("IN"));
            Connect(Component("Replace__env_var_2_"), Port("OUT"), Component("Read__site.json"), Port("IN"));
            Connect(Component("Read__sim.json"), Port("OUT"), Component("Create_sim_object"), Port("IN"));
            Connect(Component("Read__crop.json"), Port("OUT"), Component("Create_crop_object"), Port("IN"));
            Connect(Component("Read__site.json"), Port("OUT"), Component("Create_site_object"), Port("IN"));
            Connect(Component("Create_sim_object"), Port("OUT"), Component("Create__MONICA_env"), Port("SIM"));
            Connect(Component("Create_crop_object"), Port("OUT"), Component("Create__MONICA_env"), Port("CROP"));
            Connect(Component("Create_site_object"), Port("OUT"), Component("Create__MONICA_env"), Port("SITE"));
            Connect(Component("Split"), Port("OUT"), Component("Load_Balance"), Port("IN"));
            Connect(Component("Load_Balance"), Port("OUT[0]"), Component("Monica_0"), Port("LATLON"));
            Connect(Component("Load_Balance"), Port("OUT[1]"), Component("MONICA_1"), Port("LATLON"));
            Initialize("DeepClone", Component("DUP"), Port("METH"));
            Connect(Component("Create__MONICA_env"), Port("OUT"), Component("DUP"), Port("IN"));
            Connect(Component("DUP"), Port("OUT[0]"), Component("Monica_0"), Port("ENV"));
        }
    }
}
