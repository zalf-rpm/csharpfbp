﻿using System;
using FBPLib;
using System.Linq;
using Components;
using System.Threading.Tasks;
using NUnit.Framework;

namespace UnitTests
{
    public class CanReadText : Network
    {
        /* *
               * Copyright 2007, 2008, J. Paul Morrison.  At your option, you may copy, 
               * distribute, or make derivative works under the terms of the Clarified Artistic License, 
               * based on the Everything Development Company's Artistic License.  A document describing 
               * this License may be found at http://www.jpaulmorrison.com/fbp/artistic2.htm. 
               * THERE IS NO WARRANTY; USE THIS PRODUCT AT YOUR OWN RISK.
               * */

        public StoreValues StoreValues;

        public override void Define() /* throws Throwable */
        {
            // component("MONITOR", Monitor.class);

            Connect(Component("Read", typeof(ReadText)),
                Port("OUT"),
                Component("StoreValues", typeof(StoreValues)),
                Port("IN"));

            StoreValues = Component("StoreValues") as StoreValues;

            Object d = (Object)@"fake_latin.txt";
            Initialize(d,
                Component("Read"),
                Port("SOURCE"));
        }

        [Test]
        public async Task CanReadTextTest()
        {
            var network = new CanReadText();
            await network.GoA();
            var values = network.StoreValues.Values.Cast<string>().ToArray();
            Assert.AreEqual(1, values.Length);
            Assert.IsTrue(values[0].StartsWith("Lorem ipsum"));
        }

        [Test]
        public async Task MergeAndSort()
        {
            await new TestNetworks.Networks.MergeandSort().GoA();
        }

    }
}
