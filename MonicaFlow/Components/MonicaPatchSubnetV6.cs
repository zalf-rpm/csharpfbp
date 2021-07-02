using FBPLib;

namespace Components
{
    //change namespace name if desired
    [ComponentDescription("Subnet running MONICA on a patch of data")]
    [InPort("LATLON")]
    [InPort("CONF")]
    [OutPort("OUT")]
    public class MonicaPatchSubnetV6 : CapabilitySubNet
    {
        string description = "Subnet running MONICA on a patch of data";
        public override void Define()
        {
            Component("TimeSeries", typeof(ConnectToSturdyRef));
            Component("Run_Monica", typeof(RunEnvModel));
            Component("Attach_soil_profile", typeof(AttachSoilProfiles));
            Component("Attach_Rest_Env_Copies", typeof(SetCopiedAttribute));
            Component("MONICA", typeof(ConnectToSturdyRef));
            Component("Create_Env", typeof(CreateModelEnv2));
            Component("SUBOUT", typeof(SubOut));
            Initialize("OUT", Component("SUBOUT"), Port("NAME"));
            Component("Set_timeseries_cap_attr", typeof(SetCapAttribute));
            Component("Soil_service", typeof(ConnectToSturdyRef));
            Component("Replace_by__structured_text", typeof(ReplaceByStructuredTextSubnet));
            Component("SUBIN", typeof(SubIn));
            Initialize("LATLON", Component("SUBIN"), Port("NAME"));
            Component("SUBIN_2_", typeof(SubIn));
            Initialize("CONF", Component("SUBIN_2_"), Port("NAME"));
            Component("unpack_config", typeof(JsonGetValues));
            Connect(Component("Replace_by__structured_text"), Port("OUT"), Component("Set_timeseries_cap_attr"), Port("IN"));
            Initialize("time-series", Component("Set_timeseries_cap_attr"), Port("KEY"));
            Connect(Component("Set_timeseries_cap_attr"), Port("OUT"), Component("Create_Env"), Port("IN"));
            Initialize("rest", Component("Replace_by__structured_text"), Port("KEY"));
            Initialize("JSON", Component("Replace_by__structured_text"), Port("TYPE"));
            Connect(Component("Create_Env"), Port("OUT"), Component("Run_Monica"), Port("ENV"));
            Connect(Component("Attach_Rest_Env_Copies"), Port("OUT"), Component("Replace_by__structured_text"), Port("IN"));
            Initialize("EnvInstance<StructuredText,StructuredText>", Component("MONICA"), Port("TYPE"));
            Connect(Component("Attach_soil_profile"), Port("OUT"), Component("Attach_Rest_Env_Copies"), Port("IN"));
            Initialize("TimeSeries", Component("TimeSeries"), Port("TYPE"));
            Initialize("rest", Component("Attach_Rest_Env_Copies"), Port("KEY"));
            Initialize("DeepClone", Component("Attach_Rest_Env_Copies"), Port("METH"));
            Connect(Component("SUBIN"), Port("OUT"), Component("Attach_soil_profile"), Port("IN"));
            Connect(Component("Soil_service"), Port("OUT"), Component("Attach_soil_profile"), Port("CAP"));
            Initialize("sand, clay, organicCarbon, bulkDensity, soilType", Component("Attach_soil_profile"), Port("MAN"));
            Initialize("SoilService", Component("Soil_service"), Port("TYPE"));
            Connect(Component("Run_Monica"), Port("OUT"), Component("SUBOUT"), Port("IN"));
            Connect(Component("SUBIN_2_"), Port("OUT"), Component("unpack_config"), Port("IN"));
            Connect(Component("unpack_config"), Port("OUT[0]"), Component("Attach_Rest_Env_Copies"), Port("VAL"));
            Initialize("{\"0\": \"monica_json_env\", \"1\": \"soil_service_sr\", \"2\": \"climate_service_sr\", \"3\": \"monica_sr\"}", Component("unpack_config"), Port("CONF"));
            Connect(Component("unpack_config"), Port("OUT[1]"), Component("Soil_service"), Port("IN"));
            Connect(Component("TimeSeries"), Port("OUT"), Component("Set_timeseries_cap_attr"), Port("CAP"));
            Connect(Component("unpack_config"), Port("OUT[2]"), Component("TimeSeries"), Port("IN"));
            Connect(Component("MONICA"), Port("OUT"), Component("Run_Monica"), Port("CAP"));
            Connect(Component("unpack_config"), Port("OUT[3]"), Component("MONICA"), Port("IN"));
        }
    }
}