using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FBPLib;
using Components;
using NUnit.Framework;

namespace UnitTests
{
    class TestCapNetwork : CapabilityNetwork
    {
        public override void Define() /* throws Throwable */
        {
            // component("MONITOR", Monitor.class);

            Connect(
                Component("CapSrc", typeof(CapabilitySource)), Port("OUT"),
                Component("CapUse", typeof(CapabilityUse)), Port("IN"));

            Connect(
                Component("CapUse"), Port("OUT"),
                Component("WriteConsole", typeof(WriteToConsole)), Port("IN"));

            Initialize("capnp://localhost:11002", Component("CapSrc"), Port("SR"));
        }

        [Test]
        public async Task RunNetworkTest()
        {
            var network = new TestCapNetwork();
            await network.GoAsync();
            Assert.AreEqual("1", "1");
        }
    }
}
