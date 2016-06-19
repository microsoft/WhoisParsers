// -----------------------------------------------------------------------
// <copyright file="IPUtilsTests.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Utils.Tests
{
    using System;
    using System.Net;

    #if !NUNIT
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
    #else
    using NUnit.Framework;
    using TestInitialize = NUnit.Framework.SetUpAttribute;
    using TestContext = System.Object;
    using TestProperty = NUnit.Framework.PropertyAttribute;
    using TestClass = NUnit.Framework.TestFixtureAttribute;
    using TestMethod = NUnit.Framework.TestAttribute;
    using TestCleanup = NUnit.Framework.TearDownAttribute;
    #endif

    [TestClass]
    public class IPUtilsTests
    {
        [TestMethod]
        public void TestIPAddressIncrement()
        {
            // IPv4
            Assert.AreEqual(IPAddress.Parse("192.168.0.2"), IPAddress.Parse("192.168.0.1").Increment(), "Incrementing 192.168.0.1 should yield 192.168.0.2");
            Assert.AreEqual(IPAddress.Parse("0.0.0.0"), IPAddress.Parse("255.255.255.255").Increment(), "Incrementing 255.255.255.255 should yield 0.0.0.0 because it wraps around");

            // IPv6
            Assert.AreEqual(IPAddress.Parse("2001:db8:a0b:12f0::2"), IPAddress.Parse("2001:db8:a0b:12f0::1").Increment(), "Incrementing 2001:db8:a0b:12f0::1 should yield 2001:db8:a0b:12f0::2");
            Assert.AreEqual(IPAddress.Parse("0000:0000:0000:0000:0000:0000:0000:0000"), IPAddress.Parse("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff").Increment(), "Incrementing ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff should yield 0000:0000:0000:0000:0000:0000:0000:0000 because it wraps around");
            Assert.AreEqual(IPAddress.Parse("0000:0000:0000:0000:0000:0000:0000:0001"), IPAddress.Parse("0000:0000:0000:0000:0000:0000:0000:0000").Increment(), "Incrementing 0000:0000:0000:0000:0000:0000:0000:0000 should yield 0000:0000:0000:0000:0000:0000:0000:0001");
        }

        [TestMethod]
        public void TestIPAddressIncrementBy()
        {
            // IPv4
            Assert.AreEqual(IPAddress.Parse("192.168.0.11"), IPAddress.Parse("192.168.0.1").IncrementBy(10), "Incrementing 192.168.0.1 by 10 should yield 192.168.0.11");
            Assert.AreEqual(IPAddress.Parse("0.0.0.255"), IPAddress.Parse("255.255.255.255").IncrementBy(256), "Incrementing 255.255.255.255 by 255 should yield 0.0.0.254 because it wraps around");
            Assert.AreEqual(IPAddress.Parse("192.168.2.0"), IPAddress.Parse("192.168.1.0").IncrementBy(256), "Incrementing 192.168.1.0 by 256 should yield 192.168.2.0");
            Assert.AreEqual(IPAddress.Parse("192.169.1.0"), IPAddress.Parse("192.168.1.0").IncrementBy(256 * 256), "Incrementing 192.168.1.0 by 256^2 should yield 192.169.1.0");

            // IPv6
            Assert.AreEqual(IPAddress.Parse("2001:db8:a0b:12f0::6"), IPAddress.Parse("2001:db8:a0b:12f0::1").IncrementBy(5), "Incrementing 2001:db8:a0b:12f0::1 by 5 should yield 2001:db8:a0b:12f0::6");
            Assert.AreEqual(IPAddress.Parse("0000:0000:0000:0000:0000:0000:0000:00ff"), IPAddress.Parse("ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff").IncrementBy(256), "Incrementing ffff:ffff:ffff:ffff:ffff:ffff:ffff:ffff by 256 should yield 0000:0000:0000:0000:0000:0000:0000:00ff because it wraps around");
        }
    }
}
