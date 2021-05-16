using FBPLib;

namespace Components
{
    //change namespace name if desired
    [ComponentDescription("Subnet running MONICA on a patch of data")]
    [InPort("TIMESERIES_SR")]
    [InPort("MONICA_SR")]
    [InPort("SIM")]
    [InPort("CROP")]
    [InPort("SITE")]
    [OutPort("SECTIONS")]

    public class MonicaPatchSubnet : SubNet
    {
        string description = "Subnet running MONICA on a patch of data";
        public override void Define()
        {
            Component("SUBOUT", typeof(SubOut));
            Initialize("SECTIONS", Component("SUBOUT"), Port("NAME"));
            Component("TimeSeries", typeof(ConnectToSturdyRef));
            Component("Run_Monica", typeof(RunEnvModel));
            Component("MONICA", typeof(ConnectToSturdyRef));
            Component("Create_Env", typeof(CreateModelEnv));
            Component("Create_JSON__Rest_Env", typeof(CreateMonicaEnv));
            Component("Create__sim_object", typeof(CreateJsonObject));
            Component("Create__crop_object", typeof(CreateJsonObject));
            Component("Create_site_object", typeof(CreateJsonObject));
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
            Connect(Component("SUBIN_3_"), Port("OUT"), Component("Create__sim_object"), Port("IN"));
            Connect(Component("Run_Monica"), Port("OUT"), Component("SUBOUT"), Port("IN"));
            Connect(Component("MONICA"), Port("OUT"), Component("Run_Monica"), Port("CAP"));
            Connect(Component("Create_Env"), Port("ENV"), Component("Run_Monica"), Port("ENV"));
            Connect(Component("Create__sim_object"), Port("OUT"), Component("Create_JSON__Rest_Env"), Port("SIM"));
            Connect(Component("Create__crop_object"), Port("OUT"), Component("Create_JSON__Rest_Env"), Port("CROP"));
            Connect(Component("Create_site_object"), Port("OUT"), Component("Create_JSON__Rest_Env"), Port("SITE"));
            Connect(Component("Create_JSON__Rest_Env"), Port("ENV"), Component("Make__Structured__Text"), Port("IN"));
            Initialize("JSON", Component("Make__Structured__Text"), Port("STR"));
            Connect(Component("Make__Structured__Text"), Port("OUT"), Component("Create_Env"), Port("REST"));
            Initialize("EnvInstance<StructuredText,StructuredText>", Component("MONICA"), Port("CT"));
            Initialize("TimeSeries", Component("TimeSeries"), Port("CT"));
            Connect(Component("TimeSeries"), Port("OUT"), Component("Create_Env"), Port("TS"));
            Connect(Component("SUBIN"), Port("OUT"), Component("TimeSeries"), Port("SR"));
            Connect(Component("SUBIN_2_"), Port("OUT"), Component("MONICA"), Port("SR"));
            Connect(Component("SUBIN_5_"), Port("OUT"), Component("Create_site_object"), Port("IN"));
            Connect(Component("SUBIN_4_"), Port("OUT"), Component("Create__crop_object"), Port("IN"));
        }
    }
}