// -----------------------------------------------------------------------
// <copyright file="ApnicTsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System.Collections.Generic;
    using Parsers;

    public class ApnicTsvWriter : TsvWriter
    {
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
    }
}
