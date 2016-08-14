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

        public void NetworksWithLocationsToTsv(string inputFolderPath, string outputFolderPath)
        {
            if (!Directory.Exists(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
            }

            var parser = new WhoisParser(new SectionTokenizer(), new SectionParser());

            foreach (var inputFilePath in Directory.GetFiles(inputFolderPath))
            {
                var locationExtraction = new NetworkLocationExtraction(parser);
                var outputFilePath = Path.Combine(outputFolderPath, Path.GetFileName(inputFilePath));

                using (var outputFile = new StreamWriter(outputFilePath))
                {
                    foreach (var network in locationExtraction.ExtractNetworksWithLocations(inputFilePath, inputFilePath))
                    {
                        if (network.Id != null && network.Location.AddressSeemsValid())
                        {
                            var networkTsv = network.ToTsv();
                            outputFile.WriteLine(networkTsv);
                        }
                    }
                }
            }
        }
    }
}
