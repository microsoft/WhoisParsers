﻿// -----------------------------------------------------------------------
// <copyright file="LacnicTsvWriter.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Geolocation.Whois.TsvExport
{
    using System.Collections.Generic;
    using Parsers;

    public class LacnicTsvWriter : TsvWriter
    {
        public LacnicTsvWriter(IWhoisParser parser) : base(parser)
        {
        }

        public void ExportIpvRangesToTsv(string inputFilePath, string outputFilePath)
        {
            var outputColumns = new List<string> { "inetnum", "status", "owner", "city", "country", "owner-c", "tech-c", "abuse-c", "inetrev", "nserver", "created", "changed", "source", "inetnum-up" };
            this.ExportFieldsToTsv(inputFilePath: inputFilePath, outputFilePath: outputFilePath, recordType: "inetnum", outputColumns: outputColumns);
        }
    }
}
