using FBPLib;

namespace Components
{
    //change namespace name if desired
    [ComponentDescription("Subnet running MONICA on a patch of data")]
    [InPort("TIMESERIES_SR")]
    [InPort("SOILSERVICE_SR")]
    [InPort("MONICA_SR")]
    [InPort("LATLON")]
    [InPort("SIM")]
    [InPort("CROP")]
    [InPort("SITE")]
    //[OutPort("SECTIONS")] //V3
    [OutPort("OUT")]
    public class MonicaPatchSubnet : CapabilitySubNet
    {
        string description = "Subnet running MONICA on a patch of data";
        public override void Define()
        {
            //* //V4
            Component("TimeSeries", typeof(ConnectToSturdyRef));
            Component("Run_Monica", typeof(RunEnvModel));
            Component("Attach_soil_profile", typeof(AttachSoilProfiles));
            Component("Attach_Rest_Env_Copies", typeof(SetCopiedAttribute));
            Component("MONICA", typeof(ConnectToSturdyRef));
            Component("Create_Env", typeof(CreateModelEnv2));
            Component("Create_JSON_Rest_Env", typeof(CreateMonicaEnv));
            Component("Create__sim_object", typeof(CreateJsonObject));
            Component("Create__crop_object", typeof(CreateJsonObject));
            Component("Create_site_object", typeof(CreateJsonObject));
            Component("SUBIN", typeof(SubIn));
            Initialize("TIMESERIES_SR", Component("SUBIN"), Port("NAME"));
            Component("SUBIN_2_", typeof(SubIn));
            Initialize("MONICA_SR", Component("SUBIN_2_"), Port("NAME"));
            Component("SUBIN_3_", typeof(SubIn));
            Initialize("SIM", Component("SUBIN_3_"), Port("NAME"));
            Component("SUBIN_4_", typeof(SubIn));
            Initialize("CROP", Component("SUBIN_4_"), Port("NAME"));
            Component("SUBIN_5_", typeof(SubIn));
            Initialize("SITE", Component("SUBIN_5_"), Port("NAME"));
            Component("SUBOUT", typeof(SubOut));
            Initialize("OUT", Component("SUBOUT"), Port("NAME"));
            Component("Set_timeseries_cap_attr", typeof(SetCapAttribute));
            Component("Soil_service", typeof(ConnectToSturdyRef));
            Component("SUBIN_6_", typeof(SubIn));
            Initialize("SOILSERVICE_SR", Component("SUBIN_6_"), Port("NAME"));
            Component("Replace_by__structured_text", typeof(ReplaceByStructuredTextSubnet));
            Component("SUBIN_7_", typeof(SubIn));
            Initialize("LATLON", Component("SUBIN_7_"), Port("NAME"));
            Connect(Component("Replace_by__structured_text"), Port("OUT"), Component("Set_timeseries_cap_attr"), Port("IN"));
            Connect(Component("TimeSeries"), Port("OUT"), Component("Set_timeseries_cap_attr"), Port("CAP"));
            Initialize("time-series", Component("Set_timeseries_cap_attr"), Port("KEY"));
            Connect(Component("Set_timeseries_cap_attr"), Port("OUT"), Component("Create_Env"), Port("IN"));
            Connect(Component("MONICA"), Port("OUT"), Component("Run_Monica"), Port("CAP"));
            Initialize("rest", Component("Replace_by__structured_text"), Port("KEY"));
            Connect(Component("Create_Env"), Port("OUT"), Component("Run_Monica"), Port("ENV"));
            Initialize("JSON", Component("Replace_by__structured_text"), Port("TYPE"));
            Connect(Component("Attach_Rest_Env_Copies"), Port("OUT"), Component("Replace_by__structured_text"), Port("IN"));
            Initialize("EnvInstance<StructuredText,StructuredText>", Component("MONICA"), Port("TYPE"));
            Connect(Component("Attach_soil_profile"), Port("OUT"), Component("Attach_Rest_Env_Copies"), Port("IN"));
            Initialize("TimeSeries", Component("TimeSeries"), Port("TYPE"));
            Connect(Component("SUBIN"), Port("OUT"), Component("TimeSeries"), Port("IN"));
            Connect(Component("SUBIN_2_"), Port("OUT"), Component("MONICA"), Port("IN"));
            Initialize("rest", Component("Attach_Rest_Env_Copies"), Port("KEY"));
            Initialize("DeepClone", Component("Attach_Rest_Env_Copies"), Port("METH"));
            Connect(Component("Create_JSON_Rest_Env"), Port("OUT"), Component("Attach_Rest_Env_Copies"), Port("VAL"));
            Connect(Component("SUBIN_5_"), Port("OUT"), Component("Create_site_object"), Port("IN"));
            Connect(Component("SUBIN_4_"), Port("OUT"), Component("Create__crop_object"), Port("IN"));
            Connect(Component("SUBIN_3_"), Port("OUT"), Component("Create__sim_object"), Port("IN"));
            Connect(Component("SUBIN_7_"), Port("OUT"), Component("Attach_soil_profile"), Port("IN"));
            Connect(Component("SUBIN_6_"), Port("OUT"), Component("Soil_service"), Port("IN"));
            Connect(Component("Create__sim_object"), Port("OUT"), Component("Create_JSON_Rest_Env"), Port("SIM"));
            Connect(Component("Create__crop_object"), Port("OUT"), Component("Create_JSON_Rest_Env"), Port("CROP"));
            Connect(Component("Create_site_object"), Port("OUT"), Component("Create_JSON_Rest_Env"), Port("SITE"));
            Connect(Component("Soil_service"), Port("OUT"), Component("Attach_soil_profile"), Port("CAP"));
            Initialize("sand, clay, organicCarbon, bulkDensity, soilType", Component("Attach_soil_profile"), Port("MAN"));
            Initialize("SoilService", Component("Soil_service"), Port("TYPE"));
            Connect(Component("Run_Monica"), Port("OUT"), Component("SUBOUT"), Port("IN"));
            //*/

            /* //V3
            Component("TimeSeries", typeof(ConnectToSturdyRef));
            Component("Run_Monica", typeof(RunEnvModel));
            Component("Attach_soil_profile", typeof(AttachSoilProfiles));
            Component("Attach_Rest_Env_Copies", typeof(SetCopiedAttribute));
            Component("MONICA", typeof(ConnectToSturdyRef));
            Component("Create_Env", typeof(CreateModelEnv2));
            Component("Create_JSON_Rest_Env", typeof(CreateMonicaEnv));
            Component("Create__sim_object", typeof(CreateJsonObject));
            Component("Create__crop_object", typeof(CreateJsonObject));
            Component("Create_site_object", typeof(CreateJsonObject));
            Component("SUBIN", typeof(SubIn));
            Initialize("TIMESERIES_SR", Component("SUBIN"), Port("NAME"));
            Component("SUBIN_2_", typeof(SubIn));
            Initialize("MONICA_SR", Component("SUBIN_2_"), Port("NAME"));
            Component("SUBIN_3_", typeof(SubIn));
            Initialize("SIM", Component("SUBIN_3_"), Port("NAME"));
            Component("SUBIN_4_", typeof(SubIn));
            Initialize("CROP", Component("SUBIN_4_"), Port("NAME"));
            Component("SUBIN_5_", typeof(SubIn));
            Initialize("SITE", Component("SUBIN_5_"), Port("NAME"));
            Component("SUBOUT", typeof(SubOut));
            Initialize("SECTIONS", Component("SUBOUT"), Port("NAME"));
            Component("Set_timeseries_cap_attr", typeof(SetCapAttribute));
            Component("Soil_service", typeof(ConnectToSturdyRef));
            Component("SUBIN_6_", typeof(SubIn));
            Initialize("SOILSERVICE_SR", Component("SUBIN_6_"), Port("NAME"));
            Component("Replace_by__structured_text", typeof(ReplaceByStructuredTextSubnet));
            Component("SUBIN_7_", typeof(SubIn));
            Initialize("LATLON", Component("SUBIN_7_"), Port("NAME"));
            Component("to_JSON", typeof(StructuredTextToText));
            Component("Split_sections", typeof(SelectJsonElement));
            Connect(Component("Replace_by__structured_text"), Port("OUT"), Component("Set_timeseries_cap_attr"), Port("IN"));
            Connect(Component("TimeSeries"), Port("OUT"), Component("Set_timeseries_cap_attr"), Port("CAP"));
            Initialize("time-series", Component("Set_timeseries_cap_attr"), Port("KEY"));
            Connect(Component("Set_timeseries_cap_attr"), Port("OUT"), Component("Create_Env"), Port("IN"));
            Connect(Component("MONICA"), Port("OUT"), Component("Run_Monica"), Port("CAP"));
            Initialize("rest", Component("Replace_by__structured_text"), Port("KEY"));
            Connect(Component("Create_Env"), Port("OUT"), Component("Run_Monica"), Port("ENV"));
            Initialize("JSON", Component("Replace_by__structured_text"), Port("TYPE"));
            Connect(Component("Attach_Rest_Env_Copies"), Port("OUT"), Component("Replace_by__structured_text"), Port("IN"));
            Connect(Component("Run_Monica"), Port("OUT"), Component("to_JSON"), Port("IN"));
            Connect(Component("to_JSON"), Port("OUT"), Component("Split_sections"), Port("IN"));
            Connect(Component("Split_sections"), Port("OUT"), Component("SUBOUT"), Port("IN"));
            Initialize("{\"key\": \"data\", \"split\": true}", Component("Split_sections"), Port("CONF"));
            Initialize("SoilService", Component("Soil_service"), Port("TYPE"));
            Connect(Component("SUBIN_6_"), Port("OUT"), Component("Soil_service"), Port("IN"));
            Connect(Component("Soil_service"), Port("OUT"), Component("Attach_soil_profile"), Port("CAP"));
            Initialize("sand, clay, organicCarbon, bulkDensity, soilType", Component("Attach_soil_profile"), Port("MAN"));
            Connect(Component("SUBIN_7_"), Port("OUT"), Component("Attach_soil_profile"), Port("IN"));
            Initialize("EnvInstance<StructuredText,StructuredText>", Component("MONICA"), Port("TYPE"));
            Connect(Component("Attach_soil_profile"), Port("OUT"), Component("Attach_Rest_Env_Copies"), Port("IN"));
            Initialize("TimeSeries", Component("TimeSeries"), Port("TYPE"));
            Connect(Component("SUBIN_3_"), Port("OUT"), Component("Create__sim_object"), Port("IN"));
            Connect(Component("SUBIN_4_"), Port("OUT"), Component("Create__crop_object"), Port("IN"));
            Connect(Component("SUBIN"), Port("OUT"), Component("TimeSeries"), Port("IN"));
            Connect(Component("SUBIN_5_"), Port("OUT"), Component("Create_site_object"), Port("IN"));
            Connect(Component("SUBIN_2_"), Port("OUT"), Component("MONICA"), Port("IN"));
            Connect(Component("Create__sim_object"), Port("OUT"), Component("Create_JSON_Rest_Env"), Port("SIM"));
            Connect(Component("Create__crop_object"), Port("OUT"), Component("Create_JSON_Rest_Env"), Port("CROP"));
            Connect(Component("Create_site_object"), Port("OUT"), Component("Create_JSON_Rest_Env"), Port("SITE"));
            Initialize("rest", Component("Attach_Rest_Env_Copies"), Port("KEY"));
            Initialize("DeepClone", Component("Attach_Rest_Env_Copies"), Port("METH"));
            Connect(Component("Create_JSON_Rest_Env"), Port("OUT"), Component("Attach_Rest_Env_Copies"), Port("VAL"));
            //*/
        }
    }
}