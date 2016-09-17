// -----------------------------------------------------------------------
// <copyright file="TestParsers.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization.Tests
{
    using System;
    using System.Collections.Generic;

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
    public class NormalizationUtilsTests
    {
        [TestMethod]
        public void TestFindOldestDate()
        {
            Assert.IsNull(NormalizationUtils.FindOldestDate(null), "The extracted date should be null because the input text is null");
            Assert.IsNull(NormalizationUtils.FindOldestDate("	kenken@sfc.wide.ad.jp    2000021 	kenken@sfc.wide.ad.jp    2000210	kenken@sfc.wide.ad.jp    2000-0-10     "), "The extracted date should be null because the input text is invalid");

            Assert.AreEqual("2000-02-10", NormalizationUtils.FindOldestDate("	kenken@sfc.wide.ad.jp    20000210  "), "The extracted date should be 2000-02-10");
            Assert.AreEqual("2005-02-05", NormalizationUtils.FindOldestDate("ripe-dbm@ripe.net 20010724 hostmaster@ripe.net 20011024 hostmaster@ripe.net 20020805 ripe-dbm@ripe.net 20040503 ripe-dbm@ripe.net 20041229 hostmaster@afrinic.net 20050205"), "The extracted date should be 2005-02-05");
            Assert.AreEqual("1987-07-08", NormalizationUtils.FindOldestDate("1987-07-08"), "The extracted date should be 1987-07-08");
        }
    }
}
