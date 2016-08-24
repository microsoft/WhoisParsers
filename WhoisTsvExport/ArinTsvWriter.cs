// -----------------------------------------------------------------------
// <copyright file="ArinTsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System.Collections.Generic;
    using System.IO;
    using Normalization;
    using Parsers;

    public class ArinTsvWriter : TsvWriter
    {
        public ArinTsvWriter() : base(new WhoisParser(new SectionTokenizer(), new SectionParser()))
        {
        }

        public ArinTsvWriter(IWhoisParser parser) : base(parser)
        {
        }

        public void ExportOrganizationsToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "OrgID", "OrgName", "Street", "City", "State/Prov", "Country", "PostalCode", "RegDate", "Updated", "OrgAbuseHandle", "OrgAdminHandle", "OrgNOCHandle", "OrgTechHandle", "ReferralServer", "Source", "Comment" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "OrgID", outputColumns: outputColumns);
        }

        public void ExportIpv4RangesToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "NetHandle", "OrgID", "Parent", "NetName", "NetRange", "NetType", "RegDate", "Updated", "Source", "TechHandle", "AbuseHandle", "OriginAS", "NOCHandle", "Comment" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "NetHandle", outputColumns: outputColumns);
        }

        public void ExportIpv6RangesToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "V6NetHandle", "OrgID", "Parent", "NetName", "NetRange", "NetType", "RegDate", "Updated", "Source", "TechHandle", "AbuseHandle", "OriginAS", "NOCHandle", "Comment" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "V6NetHandle", outputColumns: outputColumns);
        }

        public void NetworksWithLocationsToTsv(string inputFilePath, string outputFilePath)
        {
            var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());
            this.NetworksWithLocationsToTsv(parser, inputFilePath, outputFilePath);
        }

        public void NetworksLocationPropertyCountsToTsv(string inputFilePath, string propertyName, string outputFilePath)
        {
            var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());
            this.NetworksLocationPropertyCountsToTsv(parser, inputFilePath, propertyName, outputFilePath);
        }
    }
}
