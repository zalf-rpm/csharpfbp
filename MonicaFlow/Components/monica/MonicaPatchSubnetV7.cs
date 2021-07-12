using FBPLib;

namespace Components
{
    //change namespace name if desired
    [ComponentDescription("Subnet running MONICA on a patch of data")]
    [InPort("LATLON")]
    [InPort("CONF")]
    [OutPort("OUT")]
    public class MonicaPatchSubnetV7 : CapabilitySubNet
    {
        string description = "Subnet running MONICA on a patch of data";
        public override void Define()
        {
            Component("Climate_service", typeof(ConnectToSturdyRef));
            Component("Run_Monica", typeof(RunEnvModel));
            Component("MONICA", typeof(ConnectToSturdyRef));
            Component("get_datasets", typeof(SendMessage));
            Component("select_first", typeof(SelectElement));
            Component("get_dataset", typeof(GetProperty));
            Component("Dup", typeof(CapnpDuplicate));
            Component("SUBOUT", typeof(SubOut));
            Initialize("OUT", Component("SUBOUT"), Port("NAME"));
            Component("Soil_service", typeof(ConnectToSturdyRef));
            Component("get_soil_profile", typeof(GetSoilProfiles));
            Component("create_model_env", typeof(CreateModelEnv));
            Component("SUBIN", typeof(SubIn));
            Initialize("LATLON", Component("SUBIN"), Port("NAME"));
            Component("copy_JSON_env", typeof(CopyOnTrigger));
            Component("create_structured_text", typeof(CreateStructuredText));
            Component("SUBIN_2_", typeof(SubIn));
            Initialize("CONF", Component("SUBIN_2_"), Port("NAME"));
            Component("unpack_config", typeof(JsonGetValues));
            Component("closest_timeseries", typeof(SendMessage));
            Component("set_custom_id", typeof(JsonSetValue));
            Connect(Component("get_datasets"), Port("OUT"), Component("select_first"), Port("IN"));
            Initialize("Data", Component("get_dataset"), Port("PROP"));
            Connect(Component("select_first"), Port("OUT"), Component("get_dataset"), Port("IN"));
            Connect(Component("get_dataset"), Port("OUT"), Component("closest_timeseries"), Port("IN"));
            Connect(Component("SUBIN"), Port("OUT"), Component("Dup"), Port("IN"));
            Initialize("ClosestTimeSeriesAt", Component("closest_timeseries"), Port("MSG"));
            Initialize("ClimateService", Component("Climate_service"), Port("TYPE"));
            Connect(Component("Soil_service"), Port("OUT"), Component("get_soil_profile"), Port("CAP"));
            Initialize("sand, clay, organicCarbon, bulkDensity, soilType", Component("get_soil_profile"), Port("MAN"));
            Connect(Component("Dup"), Port("OUT[1]"), Component("get_soil_profile"), Port("LATLON"));
            Connect(Component("unpack_config"), Port("OUT[0]"), Component("copy_JSON_env"), Port("COPY"));
            Connect(Component("Dup"), Port("OUT[2]"), Component("copy_JSON_env"), Port("TRIG"));
            Connect(Component("create_structured_text"), Port("OUT"), Component("create_model_env"), Port("REST"));
            Initialize("JSON", Component("create_structured_text"), Port("TYPE"));
            Initialize("DeepClone", Component("copy_JSON_env"), Port("METH"));
            Connect(Component("create_model_env"), Port("OUT"), Component("Run_Monica"), Port("ENV"));
            Connect(Component("MONICA"), Port("OUT"), Component("Run_Monica"), Port("CAP"));
            Connect(Component("unpack_config"), Port("OUT[3]"), Component("MONICA"), Port("IN"));
            Connect(Component("unpack_config"), Port("OUT[1]"), Component("Soil_service"), Port("IN"));
            Connect(Component("unpack_config"), Port("OUT[2]"), Component("Climate_service"), Port("IN"));
            Connect(Component("Dup"), Port("OUT[0]"), Component("closest_timeseries"), Port("PARAMS[0]"));
            Initialize("EnvInstance<StructuredText,StructuredText>", Component("MONICA"), Port("TYPE"));
            Connect(Component("closest_timeseries"), Port("OUT"), Component("create_model_env"), Port("CLIM"));
            Initialize("{\"0\": \"monica_json_env\", \"1\": \"soil_service_sr\", \"2\": \"climate_service_sr\", \"3\": \"monica_sr\"}", Component("unpack_config"), Port("CONF"));
            Connect(Component("get_soil_profile"), Port("OUT"), Component("create_model_env"), Port("SOIL"));
            Connect(Component("copy_JSON_env"), Port("OUT"), Component("set_custom_id"), Port("OBJ"));
            Initialize("SoilService", Component("Soil_service"), Port("TYPE"));
            Connect(Component("set_custom_id"), Port("OUT"), Component("create_structured_text"), Port("IN"));
            Connect(Component("Dup"), Port("OUT[3]"), Component("set_custom_id"), Port("VAL"));
            Connect(Component("Run_Monica"), Port("OUT"), Component("SUBOUT"), Port("IN"));
            Initialize("customId", Component("set_custom_id"), Port("KEY"));
            Connect(Component("SUBIN_2_"), Port("OUT"), Component("unpack_config"), Port("IN"));
            Initialize("GetAvailableDatasets", Component("get_datasets"), Port("MSG"));
            Connect(Component("Climate_service"), Port("OUT"), Component("get_datasets"), Port("IN"));
        }
    }
}