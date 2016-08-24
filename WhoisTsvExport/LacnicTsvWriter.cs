// -----------------------------------------------------------------------
// <copyright file="LacnicTsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System.Collections.Generic;
    using System.IO;
    using Normalization;
    using Parsers;

    public class LacnicTsvWriter : TsvWriter
    {
        public LacnicTsvWriter() : base(new WhoisParser(new SectionTokenizer(), new SectionParser()))
        {
        }

        public LacnicTsvWriter(IWhoisParser parser) : base(parser)
        {
        }

        public void ExportIpv4RangesToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "inetnum", "status", "owner", "city", "country", "owner-c", "tech-c", "abuse-c", "inetrev", "nserver", "created", "changed", "source", "inetnum-up" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "inetnum", outputColumns: outputColumns);
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
