using FBPLib;

namespace Components
{
    //change namespace name if desired
    [ComponentDescription("Replace attr by structured text in IP")]
    [InPort("IN")]
    [InPort("KEY")]
    [InPort("TYPE")]
    [OutPort("OUT")]
    public class ReplaceByStructuredTextSubnet : SubNet
    {
        string description = "Replace attr by structured text in IP";
        public override void Define()
        {
            Component("Make__Structured__Text", typeof(CreateStructuredText));
            Component("Get_rest_attr", typeof(GetAttribute));
            Component("SUBIN", typeof(SubIn));
            Initialize("IN", Component("SUBIN"), Port("NAME"));
            Component("Set_structured_text_rest_attr", typeof(SetAttribute));
            Component("SUBOUT", typeof(SubOut));
            Initialize("OUT", Component("SUBOUT"), Port("NAME"));
            Component("SUBIN_2_", typeof(SubIn));
            Initialize("KEY", Component("SUBIN_2_"), Port("NAME"));
            Component("DUP", typeof(Duplicate));
            Component("SUBIN_3_", typeof(SubIn));
            Initialize("TYPE", Component("SUBIN_3_"), Port("NAME"));
            Connect(Component("Get_rest_attr"), Port("VAL"), Component("Make__Structured__Text"), Port("IN"));
            Connect(Component("Get_rest_attr"), Port("OUT"), Component("Set_structured_text_rest_attr"), Port("IN"));
            Connect(Component("SUBIN_2_"), Port("OUT"), Component("DUP"), Port("IN"));
            Connect(Component("DUP"), Port("OUT[0]"), Component("Get_rest_attr"), Port("KEY"));
            Connect(Component("DUP"), Port("OUT[1]"), Component("Set_structured_text_rest_attr"), Port("KEY"));
            Connect(Component("SUBIN_3_"), Port("OUT"), Component("Make__Structured__Text"), Port("TYPE"));
            Connect(Component("SUBIN"), Port("OUT"), Component("Get_rest_attr"), Port("IN"));
            Connect(Component("Set_structured_text_rest_attr"), Port("OUT"), Component("SUBOUT"), Port("IN"));
            Connect(Component("Make__Structured__Text"), Port("OUT"), Component("Set_structured_text_rest_attr"), Port("VAL"));
        }
    }
}