// -----------------------------------------------------------------------
// <copyright file="TestParsers.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.Parsers.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

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
    public class TestParsers
    {
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #if !NUNIT
        [DeploymentItem("arin.sample.txt")]
        #endif
        [TestMethod]
        public void TestArinColumnTypes()
        {
            var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());
            var columns = parser.ColumnsPerTypeFromFile("arin.sample.txt");

            CollectionAssert.AreEquivalent(new List<string>() { "ASHandle", "NetHandle", "OrgID", "POCHandle" }, columns.Keys, "The sample file should contain four types: ASHandle, NetHandle, OrgID, and POCHandle");
            CollectionAssert.AreEquivalent(new List<string>() { "ASHandle", "OrgID", "ASName", "ASNumber", "RegDate", "Updated", "Source" }, columns["ASHandle"], "Columns were not extracted correctly for the ASHandle type");
            CollectionAssert.AreEquivalent(new List<string>() { "NetHandle", "OrgID", "Parent", "NetName", "NetRange", "NetType", "RegDate", "Updated", "Source" }, columns["NetHandle"], "Columns were not extracted correctly for the NetHandle type");
            CollectionAssert.AreEquivalent(new List<string>() { "OrgID", "OrgName", "CanAllocate", "Street", "City", "Country", "PostalCode", "State/Prov", "RegDate", "Updated", "OrgAdminHandle", "OrgAbuseHandle", "OrgTechHandle", "OrgNOCHandle", "Source" }, columns["OrgID"], "Columns were not extracted correctly for the OrgID type");
            CollectionAssert.AreEquivalent(new List<string>() { "POCHandle", "IsRole", "LastName", "FirstName", "Street", "City", "State/Prov", "Country", "PostalCode", "RegDate", "Updated", "OfficePhone", "Mailbox", "Source" }, columns["POCHandle"], "Columns were not extracted correctly for the POCHandle type");
        }

        #if !NUNIT
        [DeploymentItem("arin.sample.txt")]
        #endif
        [TestMethod]
        public void TestArinRetrieveRecords()
        {
            var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());
            var sections = parser.RetrieveSectionsFromFile("arin.sample.txt");

            var i = -1;

            foreach (var section in sections)
            {
                i++;

                // POCHandle: BS4-ARIN
                if (i == 6)
                {
                    Assert.AreEqual("POCHandle", section.Type);
                    Assert.AreEqual("BS4-ARIN", section.Id);

                    var records = section.Records;

                    var keys = new HashSet<string>(records.Keys);
                    var expectedKeys = new HashSet<string>() { "POCHandle", "IsRole", "LastName", "FirstName", "Street", "City", "State/Prov", "Country", "PostalCode", "RegDate", "Updated", "OfficePhone", "Mailbox", "Source" };

                    Assert.IsTrue(keys.SetEquals(expectedKeys), "The returned section keys are different than expected");

                    Assert.AreEqual("BS4-ARIN", records["POCHandle"].ToString(), "The POCHandle record had an incorrect value");
                    Assert.AreEqual("N", records["IsRole"].ToString(), "The IsRole record had an incorrect value");
                    Assert.AreEqual("Schmalhofer", records["LastName"].ToString(), "The LastName record had an incorrect value");
                    Assert.AreEqual("Beverly", records["FirstName"].ToString(), "The FirstName record had an incorrect value");
                    Assert.AreEqual("Elizabethtown", records["City"].ToString(), "The City record had an incorrect value");
                    Assert.AreEqual("PA", records["State/Prov"].ToString(), "The State/Prov record had an incorrect value");
                    Assert.AreEqual("US", records["Country"].ToString(), "The Country record had an incorrect value");
                    Assert.AreEqual("17022-2298", records["PostalCode"].ToString(), "The PostalCode record had an incorrect value");
                    Assert.AreEqual("1993-03-19", records["RegDate"].ToString(), "The RegDate record had an incorrect value");
                    Assert.AreEqual("1993-03-19", records["Updated"].ToString(), "The Updated record had an incorrect value");
                    Assert.AreEqual("+1-171-361-1434", records["OfficePhone"].ToString(), "The OfficePhone record had an incorrect value");
                    Assert.AreEqual("schmalhoferb@vax.etown.edu", records["Mailbox"].ToString(), "The Mailbox record had an incorrect value");
                    Assert.AreEqual("ARIN", records["Source"].ToString(), "The Source record had an incorrect value");

                    CollectionAssert.AreEquivalent(this.SplitTextToLines("Computer Center\r\nElizabethtown College\r\nOne Alpha Drive"), this.SplitTextToLines(records["Street"].ToString()), "The Street record had an incorrect value");
                }
            }
        }

        #if !NUNIT
        [DeploymentItem("arin.sample.txt")]
        #endif
        [TestMethod]
        public void TestArinRetrieveRecordOfType()
        {
            var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());
            var sections = parser.RetrieveSectionsFromFile("arin.sample.txt", "POCHandle");

            var i = -1;

            foreach (var section in sections)
            {
                i++;

                if (i == 0)
                {
                    // POCHandle: BS4-ARIN
                    Assert.AreEqual("POCHandle", section.Type, "The section type should be POCHandle");
                    Assert.AreEqual("BS4-ARIN", section.Id, "The section id should be BS4-ARIN");

                    var records = section.Records;

                    var keys = new HashSet<string>(records.Keys);
                    var expectedKeys = new HashSet<string>() { "POCHandle", "IsRole", "LastName", "FirstName", "Street", "City", "State/Prov", "Country", "PostalCode", "RegDate", "Updated", "OfficePhone", "Mailbox", "Source" };

                    Assert.IsTrue(keys.SetEquals(expectedKeys), "The returned section keys are different than expected");

                    Assert.AreEqual("BS4-ARIN", records["POCHandle"].ToString(), "The POCHandle record had an incorrect value");
                    Assert.AreEqual("N", records["IsRole"].ToString(), "The IsRole record had an incorrect value");
                    Assert.AreEqual("Schmalhofer", records["LastName"].ToString(), "The LastName record had an incorrect value");
                    Assert.AreEqual("Beverly", records["FirstName"].ToString(), "The FirstName record had an incorrect value");
                    Assert.AreEqual("Elizabethtown", records["City"].ToString(), "The City record had an incorrect value");
                    Assert.AreEqual("PA", records["State/Prov"].ToString(), "The State/Prov record had an incorrect value");
                    Assert.AreEqual("US", records["Country"].ToString(), "The Country record had an incorrect value");
                    Assert.AreEqual("17022-2298", records["PostalCode"].ToString(), "The PostalCode record had an incorrect value");
                    Assert.AreEqual("1993-03-19", records["RegDate"].ToString(), "The RegDate record had an incorrect value");
                    Assert.AreEqual("1993-03-19", records["Updated"].ToString(), "The Updated record had an incorrect value");
                    Assert.AreEqual("+1-171-361-1434", records["OfficePhone"].ToString(), "The OfficePhone record had an incorrect value");
                    Assert.AreEqual("schmalhoferb@vax.etown.edu", records["Mailbox"].ToString(), "The Mailbox record had an incorrect value");
                    Assert.AreEqual("ARIN", records["Source"].ToString(), "The Source record had an incorrect value");

                    CollectionAssert.AreEquivalent(this.SplitTextToLines("Computer Center\r\nElizabethtown College\r\nOne Alpha Drive"), this.SplitTextToLines(records["Street"].ToString()), "The Street record had an incorrect value");
                }
                else if (i == 1)
                {
                    // POCHandle: SM4-ARIN
                    Assert.AreEqual("POCHandle", section.Type, "The section type should be POCHandle");
                    Assert.AreEqual("SM4-ARIN", section.Id, "The section id should be SM4-ARIN");
                }
            }

            Assert.AreEqual(1, i, "Only two sections should have been extracted");
        }

        #if !NUNIT
        [DeploymentItem("afrinic.sample.txt")]
        #endif
        [TestMethod]
        public void TestAfrinicSectionTokenizer()
        {
            var parser = new WhoisParser(new AfrinicSectionTokenizer(), new SectionParser());
            var sections = parser.RetrieveSectionsFromFile("afrinic.sample.txt", "key-cert");

            var i = -1;

            foreach (var section in sections)
            {
                i++;

                if (i == 0)
                {
                    // key-cert: PGPKEY-636F8DFD
                    Assert.AreEqual("key-cert", section.Type, "The section type should be key-cert");
                    Assert.AreEqual("PGPKEY-636F8DFD", section.Id, "The section id should be PGPKEY-636F8DFD");

                    var records = section.Records;

                    var keys = new HashSet<string>(records.Keys);
                    var expectedKeys = new HashSet<string>() { "key-cert", "method", "owner", "fingerpr", "certif", "mnt-by", "changed", "source" };

                    Assert.IsTrue(keys.SetEquals(expectedKeys), "The returned section keys are different than expected");

                    Assert.AreEqual("PGPKEY-636F8DFD", records["key-cert"].ToString(), "The key-cert record had an incorrect value");
                    Assert.AreEqual("PGP", records["method"].ToString(), "The method record had an incorrect value");
                    Assert.AreEqual("Musa Stephen HONLUE <stephen.honlue@afrinic.net>", records["owner"].ToString(), "The owner record had an incorrect value");
                    Assert.AreEqual("1E30 C2A1 9170 3B46 49D1  3BF9 5639 7C64 636F 8DFD", records["fingerpr"].ToString(), "The fingerpr record had an incorrect value");
                    Assert.AreEqual("MusaMnt", records["mnt-by"].ToString(), "The mnt-by record had an incorrect value");
                    Assert.AreEqual("stephen.honlue@afrinic.net 20160301", records["changed"].ToString(), "The changed record had an incorrect value");
                    Assert.AreEqual("AFRINIC", records["source"].ToString(), "The source record had an incorrect value");

                    var certificate = records["certif"].ToString();
                    var certificateLines = this.SplitTextToLines(certificate);

                    Assert.IsTrue(certificate.StartsWith("-----BEGIN PGP PUBLIC KEY BLOCK-----", StringComparison.Ordinal), "The certif record does not start with the right value");
                    Assert.IsTrue(certificate.EndsWith("-----END PGP PUBLIC KEY BLOCK-----", StringComparison.Ordinal), "The certif record does not end with the right value");
                    Assert.IsTrue(certificate.Contains("7uGv6tGU4DDzK2D6fXdcfomgwQud6u5gW283N04VUcuzlCIdaCE/XTT1FQdD+Mlj"), "The certif record does not contain the right value");

                    var j = -1;

                    foreach (var line in certificateLines)
                    {
                        j++;
                        TestContext.WriteLine(j + "|" + line + "|");
                    }

                    Assert.AreEqual(51, certificateLines.Count, "The certif record should contain 51 lines");
                }
            }

            Assert.AreEqual(0, i, "Only one section should have been extracted");
        }

        #if !NUNIT
        [DeploymentItem("rwhois.sample.txt")]
        #endif
        [TestMethod]
        public void TestRWhoisRetrieveRecords()
        {
            var parser = new WhoisParser(new SectionTokenizer(), new RWhoisSectionParser());
            var sections = parser.RetrieveSectionsFromFile("rwhois.sample.txt");

            var i = -1;

            foreach (var section in sections)
            {
                i++;

                if (i == 0)
                {
                    // network: NET-104-169-61-0-24
                    Assert.AreEqual("network", section.Type);
                    Assert.AreEqual("NET-104-169-61-0-24", section.Id);

                    var records = section.Records;

                    var keys = new HashSet<string>(records.Keys);
                    var expectedKeys = new HashSet<string>() { "Auth-Area", "ID", "Network-Name", "IP-Network", "Org-Name;I", "Street-Address", "City", "State", "Postal-Code", "Country-Code", "Tech-Contact;I", "Admin-Contact;I", "Abuse-Contact;I", "Updated", "Updated-By", "Class-Name" };

                    Assert.IsTrue(keys.SetEquals(expectedKeys), "The returned section keys are different than expected");

                    Assert.AreEqual("104.169.0.0/16", records["Auth-Area"].ToString(), "The Auth-Area record had an incorrect value");
                    Assert.AreEqual("NET-104-169-61-0-24", records["ID"].ToString(), "The ID record had an incorrect value");
                    Assert.AreEqual("104-169-61-0-24", records["Network-Name"].ToString(), "The Network-Name record had an incorrect value");
                    Assert.AreEqual("104.169.61.0/24", records["IP-Network"].ToString(), "The IP-Network record had an incorrect value");
                    Assert.AreEqual("FIOS-D Frontier Communications Everett/Redmond WA", records["Org-Name;I"].ToString(), "The Org-Name;I record had an incorrect value");
                    Assert.AreEqual("426 E. Casino Rd", records["Street-Address"].ToString(), "The Street-Address record had an incorrect value");
                    Assert.AreEqual("Everett", records["City"].ToString(), "The City record had an incorrect value");
                    Assert.AreEqual("WA", records["State"].ToString(), "The State record had an incorrect value");
                    Assert.AreEqual("98208", records["Postal-Code"].ToString(), "The Postal-Code record had an incorrect value");
                    Assert.AreEqual("US", records["Country-Code"].ToString(), "The Country-Code record had an incorrect value");
                    Assert.AreEqual("AM100-FRTR", records["Tech-Contact;I"].ToString(), "The Tech-Contact;I record had an incorrect value");
                    Assert.AreEqual("IPADMIN-FRTR", records["Admin-Contact;I"].ToString(), "The Admin-Contact;I record had an incorrect value");
                    Assert.AreEqual("ABUSE-FRTR", records["Abuse-Contact;I"].ToString(), "The Abuse-Contact;I record had an incorrect value");
                    Assert.AreEqual("20141006", records["Updated"].ToString(), "The Updated record had an incorrect value");
                    Assert.AreEqual("ipeng@frontiernet.net", records["Updated-By"].ToString(), "The Updated-By record had an incorrect value");
                    Assert.AreEqual("network", records["Class-Name"].ToString(), "The Class-Name record had an incorrect value");
                }
                else if (i == 1)
                {
                    // network: NET-104-169-0-0-16
                    Assert.AreEqual("network", section.Type);
                    Assert.AreEqual("NET-104-169-0-0-16", section.Id);

                    var records = section.Records;

                    var keys = new HashSet<string>(records.Keys);
                    var expectedKeys = new HashSet<string>() { "Auth-Area", "ID", "Network-Name", "IP-Network", "Org-Name;I", "Street-Address", "City", "State", "Postal-Code", "Country-Code", "Tech-Contact;I", "Admin-Contact;I", "Updated", "Updated-By", "Class-Name" };

                    Assert.IsTrue(keys.SetEquals(expectedKeys), "The returned section keys are different than expected");

                    Assert.AreEqual("104.169.0.0/16", records["Auth-Area"].ToString(), "The Auth-Area record had an incorrect value");
                    Assert.AreEqual("NET-104-169-0-0-16", records["ID"].ToString(), "The ID record had an incorrect value");
                    Assert.AreEqual("104-169-0-0-16", records["Network-Name"].ToString(), "The Network-Name record had an incorrect value");
                    Assert.AreEqual("104.169.0.0/16", records["IP-Network"].ToString(), "The IP-Network record had an incorrect value");
                    Assert.AreEqual("Frontier Communications Solutions", records["Org-Name;I"].ToString(), "The Org-Name;I record had an incorrect value");
                    Assert.AreEqual("180 South Clinton Ave", records["Street-Address"].ToString(), "The Street-Address record had an incorrect value");
                    Assert.AreEqual("Rochester", records["City"].ToString(), "The City record had an incorrect value");
                    Assert.AreEqual("NY", records["State"].ToString(), "The State record had an incorrect value");
                    Assert.AreEqual("14646", records["Postal-Code"].ToString(), "The Postal-Code record had an incorrect value");
                    Assert.AreEqual("US", records["Country-Code"].ToString(), "The Country-Code record had an incorrect value");
                    Assert.AreEqual("ABUSE-FRTR", records["Tech-Contact;I"].ToString(), "The Tech-Contact;I record had an incorrect value");
                    Assert.AreEqual("IPADMIN-FRTR", records["Admin-Contact;I"].ToString(), "The Admin-Contact;I record had an incorrect value");
                    Assert.AreEqual("20140813", records["Updated"].ToString(), "The Updated record had an incorrect value");
                    Assert.AreEqual("ipeng@frontiernet.net", records["Updated-By"].ToString(), "The Updated-By record had an incorrect value");
                    Assert.AreEqual("network", records["Class-Name"].ToString(), "The Class-Name record had an incorrect value");
                }
            }
        }

        private List<string> SplitTextToLines(string text)
        {
            var ret = new List<string>();

            using (var sr = new StringReader(text))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    ret.Add(line);
                }
            }

            return ret;
        }
    }
}
