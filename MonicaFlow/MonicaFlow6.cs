using System;
using FBPLib;
using Components;
using System.Threading.Tasks;

namespace MonicaFlow
{ 
    public class MonicaFlow6 : CapabilityNetwork
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
            Component("Read__sim.json", typeof(ReadCompleteFile));
            Component("Read__crop.json", typeof(ReadCompleteFile));
            Component("Read__site.json", typeof(ReadCompleteFile));
            Component("Monica_0", typeof(MonicaPatchSubnetV6));
            Component("Split", typeof(SplitString));
            Component("create_csvs", typeof(CreateMonicaCSV));
            Component("Create_JSON", typeof(CreateJsonObject));
            Component("add_env", typeof(JsonSetValue));
            Component("Monica_2", typeof(MonicaPatchSubnetV6));
            Component("Replace__env_var", typeof(ReplaceEnvVars));
            Component("Replace___env_var", typeof(ReplaceEnvVars));
            Component("Replace__env_var_2_", typeof(ReplaceEnvVars));
            Component("Load_Balance", typeof(LoadBalance));
            Component("Monica_1", typeof(MonicaPatchSubnetV6));
            Connect(Component("DUP"), Port("OUT[1]"), Component("Monica_1"), Port("CONF"));
            Connect(Component("Monica_1"), Port("OUT"), Component("create_csvs"), Port("IN"));
            Connect(Component("Replace__env_var"), Port("OUT"), Component("Read__sim.json"), Port("IN"));
            Connect(Component("Replace___env_var"), Port("OUT"), Component("Read__crop.json"), Port("IN"));
            Connect(Component("Replace__env_var_2_"), Port("OUT"), Component("Read__site.json"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\sim-min-2.json", Component("Replace__env_var"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\crop-min.json", Component("Replace___env_var"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\site-min.json", Component("Replace__env_var_2_"), Port("IN"));
            Initialize("{\"climate_service_sr\": \"capnp://localhost:11002\", \"monica_sr\": \"capnp://localhost:6666\", \"soil_service_sr\": \"capnp://localhost:10000\"}", Component("Create_JSON"), Port("IN"));
            Connect(Component("Create_JSON"), Port("OUT"), Component("add_env"), Port("OBJ"));
            Connect(Component("Create__MONICA_env"), Port("OUT"), Component("add_env"), Port("VAL"));
            Initialize("monica_json_env", Component("add_env"), Port("KEY"));
            Initialize("{\"split-at\": \"|\"}", Component("Split"), Port("CONF"));
            Connect(Component("add_env"), Port("OUT"), Component("DUP"), Port("IN"));
            Initialize("55.0329, 8.3443 | 55.0339, 8.4251 | 54.9866, 8.3462 | 54.9402, 8.348 | 54.8927, 8.2693 | 54.8938, 8.3498 | 54.8948, 8.4302 | 54.8958, 8.5107 | 54.8967, 8.5912 | 54.8975, 8.6717 | 54.8983, 8.7522 | 54.8991, 8.8327 | 54.8998, 8.9131", Component("Split"), Port("IN"));
            Initialize("DeepClone", Component("DUP"), Port("METH"));
            Connect(Component("DUP"), Port("OUT[2]"), Component("Monica_2"), Port("CONF"));
            Connect(Component("Load_Balance"), Port("OUT[2]"), Component("Monica_2"), Port("LATLON"));
            Connect(Component("Monica_2"), Port("OUT"), Component("create_csvs"), Port("IN"));
            Initialize("{\"split\": true, \"csvSep\": \";\"}", Component("create_csvs"), Port("OPTS"));
            Connect(Component("Monica_0"), Port("OUT"), Component("create_csvs"), Port("IN"));
            Connect(Component("create_csvs"), Port("OUT"), Component("WriteConsole"), Port("IN"));
            Connect(Component("Read__sim.json"), Port("OUT"), Component("Create_sim_object"), Port("IN"));
            Connect(Component("Read__crop.json"), Port("OUT"), Component("Create_crop_object"), Port("IN"));
            Connect(Component("Read__site.json"), Port("OUT"), Component("Create_site_object"), Port("IN"));
            Connect(Component("Create_sim_object"), Port("OUT"), Component("Create__MONICA_env"), Port("SIM"));
            Connect(Component("Create_crop_object"), Port("OUT"), Component("Create__MONICA_env"), Port("CROP"));
            Connect(Component("Create_site_object"), Port("OUT"), Component("Create__MONICA_env"), Port("SITE"));
            Connect(Component("Split"), Port("OUT"), Component("Load_Balance"), Port("IN"));
            Connect(Component("Load_Balance"), Port("OUT[0]"), Component("Monica_0"), Port("LATLON"));
            Connect(Component("Load_Balance"), Port("OUT[1]"), Component("Monica_1"), Port("LATLON"));
            Connect(Component("DUP"), Port("OUT[0]"), Component("Monica_0"), Port("CONF"));
        }
    }
}
