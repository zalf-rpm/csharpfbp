using System;
using System.Threading.Tasks;

namespace MonicaFlow
{ 
    public class Program
    {
        static async Task Main(String[] argv)
        {
            //AppContext.SetSwitch("Tracing", true);
            //using var network = new MonicaFlow();
            using var network = new MonicaFlow6();
            //using var network = new ZmqTestFlow();
            await network.GoAsync();
        }
    }
}
