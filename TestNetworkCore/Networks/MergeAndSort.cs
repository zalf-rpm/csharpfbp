using FBPLib;
using Components;

namespace TestNetworks.Networks

{    // change this if you want 
    public class MergeandSort : Network
    {
        string description = "Merge and Sort Network";
        public override void Define()
        {
            Component("Write_text__to_pane", typeof(WriteToConsole));
            Component("Sort", typeof(Sort));
            Component("Generate____1st_group", typeof(GenerateTestData));
            Component("Generate___2nd_group", typeof(GenerateTestData));
            Connect(Component("Sort"), Port("OUT"), Component("Write_text__to_pane"), Port("IN"));
            Connect(Component("Generate___2nd_group"), Port("OUT"), Component("Sort"), Port("IN"));
            Connect(Component("Generate____1st_group"), Port("OUT"), Component("Sort"), Port("IN"));
            Initialize("100", Component("Generate___2nd_group"), Port("COUNT"));
            Initialize("100", Component("Generate____1st_group"), Port("COUNT"));

        }
        static void main(string[] argv)
        {
            new MergeandSort().Go();
        }
    }
}
