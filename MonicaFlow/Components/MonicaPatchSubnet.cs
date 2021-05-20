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
    [OutPort("SECTIONS")]
    public class MonicaPatchSubnet : CapabilitySubNet
    {
        string description = "Subnet running MONICA on a patch of data";
        public override void Define()
        {
            Component("TimeSeries", typeof(ConnectToSturdyRef));
            Component("Run_Monica", typeof(RunEnvModel));
            Component("MONICA", typeof(ConnectToSturdyRef));
            Component("Create_Env", typeof(CreateModelEnv));
            Component("Create_JSON__Rest_Env", typeof(CreateMonicaEnv));
            Component("Create___sim_object", typeof(CreateJsonObject));
            Component("Create___crop_object", typeof(CreateJsonObject));
            Component("Create__site_object", typeof(CreateJsonObject));
            Component("Make__Structured__Text", typeof(CreateStructuredText));
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
            Component("Soil_service", typeof(ConnectToSturdyRef));
            Component("SUBIN_6_", typeof(SubIn));
            Initialize("SOILSERVICE_SR", Component("SUBIN_6_"), Port("NAME"));
            Component("SUBIN_7_", typeof(SubIn));
            Initialize("LATLON", Component("SUBIN_7_"), Port("NAME"));
            Component("Get_soil_profile", typeof(GetSoilProfiles));
            Connect(Component("SUBIN_3_"), Port("OUT"), Component("Create___sim_object"), Port("IN"));
            Connect(Component("Run_Monica"), Port("OUT"), Component("SUBOUT"), Port("IN"));
            Connect(Component("SUBIN_6_"), Port("OUT"), Component("Soil_service"), Port("SR"));
            Initialize("SoilService", Component("Soil_service"), Port("CT"));
            Connect(Component("Soil_service"), Port("OUT"), Component("Get_soil_profile"), Port("CAP"));
            Connect(Component("Get_soil_profile"), Port("OUT"), Component("Create_Env"), Port("SP"));
            Connect(Component("SUBIN_7_"), Port("OUT"), Component("Get_soil_profile"), Port("LATLON"));
            Initialize("sand, clay, organicCarbon, bulkDensity, soilType", Component("Get_soil_profile"), Port("MAN"));
            Connect(Component("MONICA"), Port("OUT"), Component("Run_Monica"), Port("CAP"));
            Connect(Component("Create_Env"), Port("OUT"), Component("Run_Monica"), Port("ENV"));
            Connect(Component("Create___sim_object"), Port("OUT"), Component("Create_JSON__Rest_Env"), Port("SIM"));
            Connect(Component("Create___crop_object"), Port("OUT"), Component("Create_JSON__Rest_Env"), Port("CROP"));
            Connect(Component("Create__site_object"), Port("OUT"), Component("Create_JSON__Rest_Env"), Port("SITE"));
            Connect(Component("Create_JSON__Rest_Env"), Port("OUT"), Component("Make__Structured__Text"), Port("IN"));
            Initialize("JSON", Component("Make__Structured__Text"), Port("STR"));
            Connect(Component("Make__Structured__Text"), Port("OUT"), Component("Create_Env"), Port("REST"));
            Initialize("EnvInstance<StructuredText,StructuredText>", Component("MONICA"), Port("CT"));
            Initialize("TimeSeries", Component("TimeSeries"), Port("CT"));
            Connect(Component("TimeSeries"), Port("OUT"), Component("Create_Env"), Port("TS"));
            Connect(Component("SUBIN"), Port("OUT"), Component("TimeSeries"), Port("SR"));
            Connect(Component("SUBIN_2_"), Port("OUT"), Component("MONICA"), Port("SR"));
            Connect(Component("SUBIN_5_"), Port("OUT"), Component("Create__site_object"), Port("IN"));
            Connect(Component("SUBIN_4_"), Port("OUT"), Component("Create___crop_object"), Port("IN"));
        }
    }
}