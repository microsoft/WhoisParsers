// -----------------------------------------------------------------------
// <copyright file="TestParsers.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Normalization.Tests
{
    using VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NormalizationUtilsTests
    {
        [TestMethod]
        public void TestFindOldestDateOptimized()
        {
            Assert.IsNull(NormalizationUtils.FindOldestDateOptimized(null), "The extracted date should be null because the input text is null");
            Assert.IsNull(NormalizationUtils.FindOldestDateOptimized("	kenken@sfc.wide.ad.jp    2000021 	kenken@sfc.wide.ad.jp    2000210	kenken@sfc.wide.ad.jp    2000-0-10     "), "The extracted date should be null because the input text is invalid");
            Assert.IsNull(NormalizationUtils.FindOldestDateOptimized("20020229"), "The extracted date should be null because the input date is invalid");

            Assert.AreEqual("2000-02-10", NormalizationUtils.FindOldestDateOptimized("	kenken@sfc.wide.ad.jp    20000210  "), "The extracted date should be 2000-02-10");
            Assert.AreEqual("2005-02-05", NormalizationUtils.FindOldestDateOptimized("ripe-dbm@ripe.net 20010724 hostmaster@ripe.net 20011024 hostmaster@ripe.net 20020805 ripe-dbm@ripe.net 20040503 ripe-dbm@ripe.net 20041229 hostmaster@afrinic.net 20050205"), "The extracted date should be 2005-02-05");
            Assert.AreEqual("1987-07-08", NormalizationUtils.FindOldestDateOptimized("1987-07-08"), "The extracted date should be 1987-07-08");
            Assert.AreEqual("2004-12-14", NormalizationUtils.FindOldestDateOptimized("rfuller @nla.gov.au 20030521 hm - changed@apnic.net 20040906 hm - changed@apnic.net 20041214"), "The extracted date should be 2004-12-14");
        }

        [TestMethod]
        public void TestFindOldestDateSlow()
        {
            Assert.IsNull(NormalizationUtils.FindOldestDateSlow(null), "The extracted date should be null because the input text is null");
            Assert.IsNull(NormalizationUtils.FindOldestDateSlow("	kenken@sfc.wide.ad.jp    2000021 	kenken@sfc.wide.ad.jp    2000210	kenken@sfc.wide.ad.jp    2000-0-10     "), "The extracted date should be null because the input text is invalid");
            Assert.IsNull(NormalizationUtils.FindOldestDateSlow("20020229"), "The extracted date should be null because the input date is invalid");

            Assert.AreEqual("2000-02-10", NormalizationUtils.FindOldestDateSlow("	kenken@sfc.wide.ad.jp    20000210  "), "The extracted date should be 2000-02-10");
            Assert.AreEqual("2005-02-05", NormalizationUtils.FindOldestDateSlow("ripe-dbm@ripe.net 20010724 hostmaster@ripe.net 20011024 hostmaster@ripe.net 20020805 ripe-dbm@ripe.net 20040503 ripe-dbm@ripe.net 20041229 hostmaster@afrinic.net 20050205"), "The extracted date should be 2005-02-05");
            Assert.AreEqual("1987-07-08", NormalizationUtils.FindOldestDateSlow("1987-07-08"), "The extracted date should be 1987-07-08");
            Assert.AreEqual("2004-12-14", NormalizationUtils.FindOldestDateSlow("rfuller @nla.gov.au 20030521 hm - changed@apnic.net 20040906 hm - changed@apnic.net 20041214"), "The extracted date should be 2004-12-14");
        }
    }
}
