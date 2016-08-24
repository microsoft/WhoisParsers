// -----------------------------------------------------------------------
// <copyright file="ApnicTsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System.Collections.Generic;
    using System.IO;
    using Normalization;
    using Parsers;

    public class ApnicTsvWriter : TsvWriter
    {
        public ApnicTsvWriter() : base(new WhoisParser(new SectionTokenizer(), new SectionParser()))
        {
        }

        public ApnicTsvWriter(IWhoisParser parser) : base(parser)
        {
        }

        public void ExportIpv4RangesToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "inetnum", "netname", "country", "descr", "admin-c", "tech-c", "status", "mnt-by", "mnt-routes", "changed", "source", "notify", "remarks", "mnt-irt", "mnt-lower", "geoloc", "language" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "inetnum", outputColumns: outputColumns);
        }

        public void ExportIpv6RangesToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "inet6num", "netname", "descr", "country", "admin-c", "tech-c", "status", "remarks", "notify", "mnt-by", "mnt-lower", "changed", "source", "mnt-irt", "mnt-routes", "geoloc", "language" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "inet6num", outputColumns: outputColumns);
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
