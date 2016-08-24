// -----------------------------------------------------------------------
// <copyright file="AfrinicTsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System.Collections.Generic;
    using System.IO;
    using Normalization;
    using Parsers;

    public class AfrinicTsvWriter : TsvWriter
    {
        public AfrinicTsvWriter() : base(new WhoisParser(new AfrinicSectionTokenizer(), new SectionParser()))
        {
        }

        public AfrinicTsvWriter(IWhoisParser parser) : base(parser)
        {
        }

        public void ExportOrganizationsToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "organisation", "org-name", "org-type", "country", "address", "remarks", "e-mail", "admin-c", "tech-c", "mnt-ref", "mnt-by", "changed", "source", "phone", "fax-no", "notify", "descr", "abuse-mailbox" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "organisation", outputColumns: outputColumns);
        }

        public void ExportIncidentResponseTeamsToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "irt", "address", "e-mail", "abuse-mailbox", "admin-c", "tech-c", "auth", "mnt-by", "changed", "source", "signature", "encryption", "phone", "irt-nfy", "notify", "org", "remarks" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "irt", outputColumns: outputColumns);
        }

        public void ExportIpv4RangesToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "inetnum", "netname", "descr", "country", "org", "admin-c", "tech-c", "status", "remarks", "mnt-by", "mnt-lower", "changed", "source", "notify", "mnt-domains", "mnt-routes", "parent", "mnt-irt" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "inetnum", outputColumns: outputColumns);
        }

        public void ExportIpv6RangesToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "inet6num", "netname", "descr", "country", "org", "admin-c", "tech-c", "mnt-by", "mnt-lower", "status", "remarks", "changed", "source", "notify", "mnt-routes", "parent", "mnt-domains", "mnt-irt" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "inet6num", outputColumns: outputColumns);
        }

        public void NetworksWithLocationsToTsv(string inputFilePath, string outputFilePath)
        {
            var parser = new WhoisParser(new AfrinicSectionTokenizer(), new SectionParser());
            this.NetworksWithLocationsToTsv(parser, inputFilePath, outputFilePath);
        }

        public void NetworksLocationPropertyCountsToTsv(string inputFilePath, string propertyName, string outputFilePath)
        {
            var parser = new WhoisParser(new AfrinicSectionTokenizer(), new SectionParser());
            this.NetworksLocationPropertyCountsToTsv(parser, inputFilePath, propertyName, outputFilePath);
        }
    }
}
