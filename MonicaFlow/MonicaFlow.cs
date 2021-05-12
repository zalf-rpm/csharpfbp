using System;
using FBPLib;
using Components;
using System.Threading.Tasks;

namespace MonicaFlow
{ 
    public class MonicaFlow : CapabilityNetwork
    {
        string description = "Click anywhere on selection area";
        public override void Define()
        {
            Component("TimeSeries", typeof(ConnectToSturdyRef));
            Component("Run_Monica", typeof(RunEnvModel));
            Component("WriteConsole", typeof(WriteToConsole));
            Component("MONICA", typeof(ConnectToSturdyRef));
            Component("Create_Env", typeof(CreateModelEnv));
            Component("Create_JSON_Rest_Env", typeof(CreateMonicaEnv));
            Component("Read_sim.json", typeof(ReadJsonObject));
            Component("Read_crop.json", typeof(ReadJsonObject));
            Component("Read_site.json", typeof(ReadJsonObject));
            Component("Make_Structured_Text", typeof(CreateStructuredText));
            Connect(Component("Run_Monica"), Port("OUT"), Component("WriteConsole"), Port("IN"));
            Initialize("capnp://localhost:11002", Component("TimeSeries"), Port("SR"));
            Initialize("TimeSeries", Component("TimeSeries"), Port("CT"));
            Initialize("capnp://localhost:6666", Component("MONICA"), Port("SR"));
            Connect(Component("MONICA"), Port("OUT"), Component("Run_Monica"), Port("CAP"));
            Connect(Component("Create_Env"), Port("ENV"), Component("Run_Monica"), Port("ENV"));
            Connect(Component("TimeSeries"), Port("OUT"), Component("Create_Env"), Port("TS"));
            Connect(Component("Read_sim.json"), Port("OUT"), Component("Create_JSON_Rest_Env"), Port("SIM"));
            Connect(Component("Read_crop.json"), Port("OUT"), Component("Create_JSON_Rest_Env"), Port("CROP"));
            Connect(Component("Read_site.json"), Port("OUT"), Component("Create_JSON_Rest_Env"), Port("SITE"));
            Initialize("C:\\Users\\micha\\MONICA\\Examples\\Hohenfinow2\\sim-min.json", Component("Read_sim.json"), Port("IN"));
            Initialize("C:\\Users\\micha\\MONICA\\Examples\\Hohenfinow2\\crop-min.json", Component("Read_crop.json"), Port("IN"));
            Initialize("C:\\Users\\micha\\MONICA\\Examples\\Hohenfinow2\\site-min.json", Component("Read_site.json"), Port("IN"));
            Connect(Component("Create_JSON_Rest_Env"), Port("ENV"), Component("Make_Structured_Text"), Port("IN"));
            Initialize("JSON", Component("Make_Structured_Text"), Port("STR"));
            Connect(Component("Make_Structured_Text"), Port("OUT"), Component("Create_Env"), Port("REST"));
            Initialize("EnvInstance<StructuredText,StructuredText>", Component("MONICA"), Port("CT"));
        }

        static async Task Main(String[] argv)
        {
            using var network = new MonicaFlow();
            await network.GoAsync();
            Console.WriteLine("bla");
        }
    }
}
