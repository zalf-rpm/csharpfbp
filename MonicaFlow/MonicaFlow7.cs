using System;
using FBPLib;
using Components;
using System.Threading.Tasks;

namespace MonicaFlow
{ 
    public class MonicaFlow7 : CapabilityNetwork
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
            Component("Monica_0", typeof(MonicaPatchSubnetV7));
            Component("create_csvs", typeof(MonicaCreateCSV));
            Component("Create_JSON", typeof(CreateJsonObject));
            Component("add_env", typeof(JsonSetValue));
            Component("Monica_2", typeof(MonicaPatchSubnetV7));
            Component("Create_JSON_array", typeof(JsonCreate));
            Component("Read_rowcol_to_latlon", typeof(ReadCompleteFile));
            Component("split_to_rowcol/latlon_array", typeof(SplitCollection));
            Component("Replace__env_var", typeof(ReplaceEnvVars));
            Component("split_to_rowcol_and_latlon", typeof(SplitCollection));
            Component("Replace___env_var", typeof(ReplaceEnvVars));
            Component("drop_row/col", typeof(Discard));
            Component("Replace__env_var_2_", typeof(ReplaceEnvVars));
            Component("split_into_lat_&_lon", typeof(SplitCollection));
            Component("create_capnp_LatLonCoord", typeof(CreateLatLonCoord));
            Component("write_file", typeof(WriteText));
            Component("get_customId", typeof(GetAttribute));
            Component("create_file_name", typeof(TextInterpolate));
            Component("Load_Balance", typeof(LoadBalance));
            Component("Monica_1", typeof(MonicaPatchSubnetV7));
            Connect(Component("DUP"), Port("OUT[1]"), Component("Monica_1"), Port("CONF"));
            Connect(Component("Monica_1"), Port("OUT"), Component("create_csvs"), Port("IN"));
            Connect(Component("Replace__env_var"), Port("OUT"), Component("Read__sim.json"), Port("IN"));
            Connect(Component("Replace___env_var"), Port("OUT"), Component("Read__crop.json"), Port("IN"));
            Connect(Component("Replace__env_var_2_"), Port("OUT"), Component("Read__site.json"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\sim-min-2.json", Component("Replace__env_var"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\crop-min.json", Component("Replace___env_var"), Port("IN"));
            Initialize("${MONICA_HOME}\\Examples\\Hohenfinow2\\site-min.json", Component("Replace__env_var_2_"), Port("IN"));
            Initialize("{\"climate_service_sr\": \"capnp://login01.cluster.zalf.de:9999\", \"monica_sr\": \"capnp://localhost:6666\", \"soil_service_sr\": \"capnp://localhost:10000\"}", Component("Create_JSON"), Port("IN"));
            Connect(Component("Create_JSON"), Port("OUT"), Component("add_env"), Port("OBJ"));
            Connect(Component("Create__MONICA_env"), Port("OUT"), Component("add_env"), Port("VAL"));
            Initialize("monica_json_env", Component("add_env"), Port("KEY"));
            Connect(Component("add_env"), Port("OUT"), Component("DUP"), Port("IN"));
            Initialize("DeepClone", Component("DUP"), Port("METH"));
            Connect(Component("DUP"), Port("OUT[2]"), Component("Monica_2"), Port("CONF"));
            Connect(Component("Load_Balance"), Port("OUT[2]"), Component("Monica_2"), Port("LATLON"));
            Connect(Component("Monica_2"), Port("OUT"), Component("create_csvs"), Port("IN"));
            Initialize("C:\\Users\\micha\\GitHub\\csharpfbp\\MonicaFlow\\rowcol-to-latlon.json", Component("Read_rowcol_to_latlon"), Port("IN"));
            Connect(Component("Read_rowcol_to_latlon"), Port("OUT"), Component("Create_JSON_array"), Port("IN"));
            Connect(Component("Create_JSON_array"), Port("OUT"), Component("split_to_rowcol/latlon_array"), Port("IN"));
            Initialize("{\"split\": true, \"csvSep\": \";\"}", Component("create_csvs"), Port("OPTS"));
            Connect(Component("split_to_rowcol/latlon_array"), Port("OUT[0]"), Component("split_to_rowcol_and_latlon"), Port("IN"));
            Connect(Component("Monica_0"), Port("OUT"), Component("create_csvs"), Port("IN"));
            Connect(Component("split_to_rowcol_and_latlon"), Port("OUT[0]"), Component("drop_row/col"), Port("IN"));
            Connect(Component("split_to_rowcol_and_latlon"), Port("OUT[1]"), Component("split_into_lat_&_lon"), Port("IN"));
            Connect(Component("split_into_lat_&_lon"), Port("OUT[0]"), Component("create_capnp_LatLonCoord"), Port("LAT"));
            Connect(Component("split_into_lat_&_lon"), Port("OUT[1]"), Component("create_capnp_LatLonCoord"), Port("LON"));
            Connect(Component("create_capnp_LatLonCoord"), Port("OUT"), Component("Load_Balance"), Port("IN"));
            Connect(Component("write_file"), Port("OUT"), Component("WriteConsole"), Port("IN"));
            Connect(Component("get_customId"), Port("OUT"), Component("write_file"), Port("IN"));
            Connect(Component("create_csvs"), Port("OUT"), Component("get_customId"), Port("IN"));
            Connect(Component("Read__sim.json"), Port("OUT"), Component("Create_sim_object"), Port("IN"));
            Initialize("customId", Component("get_customId"), Port("KEY"));
            Connect(Component("Read__crop.json"), Port("OUT"), Component("Create_crop_object"), Port("IN"));
            Connect(Component("get_customId"), Port("OUT"), Component("create_file_name"), Port("IN"));
            Connect(Component("Read__site.json"), Port("OUT"), Component("Create_site_object"), Port("IN"));
            Connect(Component("create_file_name"), Port("OUT"), Component("write_file"), Port("DESTINATION"));
            Connect(Component("Create_sim_object"), Port("OUT"), Component("Create__MONICA_env"), Port("SIM"));
            Initialize("C:\\Users\\micha\\GitHub\\csharpfbp\\MonicaFlow\\out\\out_${IN}.csv", Component("create_file_name"), Port("TEMP"));
            Connect(Component("Create_crop_object"), Port("OUT"), Component("Create__MONICA_env"), Port("CROP"));
            Connect(Component("Create_site_object"), Port("OUT"), Component("Create__MONICA_env"), Port("SITE"));
            Connect(Component("Load_Balance"), Port("OUT[0]"), Component("Monica_0"), Port("LATLON"));
            Connect(Component("Load_Balance"), Port("OUT[1]"), Component("Monica_1"), Port("LATLON"));
            Connect(Component("DUP"), Port("OUT[0]"), Component("Monica_0"), Port("CONF"));
        }
    }
}
