using FBPLib;

namespace Components
{
    //change namespace name if desired
    [ComponentDescription("Subnet running MONICA on a patch of data")]
    [InPort("TIMESERIES_SR")]
    [InPort("SOILSERVICE_SR")]
    [InPort("MONICA_SR")]
    [InPort("LATLON")]
    [InPort("ENV")]
    [OutPort("OUT")]
    public class MonicaPatchSubnetV5 : CapabilitySubNet
    {
        string description = "Subnet running MONICA on a patch of data";
        public override void Define()
        {
            //* 
            Component("TimeSeries", typeof(ConnectToSturdyRef));
            Component("Run_Monica", typeof(RunEnvModel));
            Component("Attach_soil_profile", typeof(AttachSoilProfiles));
            Component("Attach_Rest_Env_Copies", typeof(SetCopiedAttribute));
            Component("MONICA", typeof(ConnectToSturdyRef));
            Component("Create_Env", typeof(CreateModelEnv2));
            Component("SUBIN", typeof(SubIn));
            Initialize("TIMESERIES_SR", Component("SUBIN"), Port("NAME"));
            Component("SUBIN_2_", typeof(SubIn));
            Initialize("MONICA_SR", Component("SUBIN_2_"), Port("NAME"));
            Component("SUBOUT", typeof(SubOut));
            Initialize("OUT", Component("SUBOUT"), Port("NAME"));
            Component("Set_timeseries_cap_attr", typeof(SetCapAttribute));
            Component("Soil_service", typeof(ConnectToSturdyRef));
            Component("SUBIN_3_", typeof(SubIn));
            Initialize("SOILSERVICE_SR", Component("SUBIN_3_"), Port("NAME"));
            Component("Replace_by__structured_text", typeof(ReplaceByStructuredTextSubnet));
            Component("SUBIN_4_", typeof(SubIn));
            Initialize("LATLON", Component("SUBIN_4_"), Port("NAME"));
            Component("SUBIN_5_", typeof(SubIn));
            Initialize("ENV", Component("SUBIN_5_"), Port("NAME"));
            Initialize("rest", Component("Attach_Rest_Env_Copies"), Port("KEY"));
            Initialize("DeepClone", Component("Attach_Rest_Env_Copies"), Port("METH"));
            Connect(Component("Replace_by__structured_text"), Port("OUT"), Component("Set_timeseries_cap_attr"), Port("IN"));
            Connect(Component("TimeSeries"), Port("OUT"), Component("Set_timeseries_cap_attr"), Port("CAP"));
            Connect(Component("SUBIN_4_"), Port("OUT"), Component("Attach_soil_profile"), Port("IN"));
            Initialize("time-series", Component("Set_timeseries_cap_attr"), Port("KEY"));
            Connect(Component("SUBIN_3_"), Port("OUT"), Component("Soil_service"), Port("IN"));
            Connect(Component("Set_timeseries_cap_attr"), Port("OUT"), Component("Create_Env"), Port("IN"));
            Connect(Component("Soil_service"), Port("OUT"), Component("Attach_soil_profile"), Port("CAP"));
            Connect(Component("MONICA"), Port("OUT"), Component("Run_Monica"), Port("CAP"));
            Initialize("rest", Component("Replace_by__structured_text"), Port("KEY"));
            Initialize("sand, clay, organicCarbon, bulkDensity, soilType", Component("Attach_soil_profile"), Port("MAN"));
            Connect(Component("Create_Env"), Port("OUT"), Component("Run_Monica"), Port("ENV"));
            Initialize("JSON", Component("Replace_by__structured_text"), Port("TYPE"));
            Initialize("SoilService", Component("Soil_service"), Port("TYPE"));
            Connect(Component("Attach_Rest_Env_Copies"), Port("OUT"), Component("Replace_by__structured_text"), Port("IN"));
            Connect(Component("Run_Monica"), Port("OUT"), Component("SUBOUT"), Port("IN"));
            Connect(Component("SUBIN_5_"), Port("OUT"), Component("Attach_Rest_Env_Copies"), Port("VAL"));
            Initialize("EnvInstance<StructuredText,StructuredText>", Component("MONICA"), Port("TYPE"));
            Connect(Component("Attach_soil_profile"), Port("OUT"), Component("Attach_Rest_Env_Copies"), Port("IN"));
            Initialize("TimeSeries", Component("TimeSeries"), Port("TYPE"));
            Connect(Component("SUBIN"), Port("OUT"), Component("TimeSeries"), Port("IN"));
            Connect(Component("SUBIN_2_"), Port("OUT"), Component("MONICA"), Port("IN"));
            //*/
        }
    }
}